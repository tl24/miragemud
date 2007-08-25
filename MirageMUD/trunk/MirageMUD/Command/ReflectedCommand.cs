using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mirage.Data;
using Mirage.Communication;
using Mirage.Data.Query;
using log4net;


namespace Mirage.Command
{
    /// <summary>
    /// Helper class for holding Attributes about a Command and invoking it.
    /// </summary>
    public class ReflectedCommand : CommandBase, ICommand
    {
        private static ILog logger = LogManager.GetLogger(typeof(ReflectedCommand));

        #region Member Variables

        private string _description;
        private MethodInfo _methodInfo;

        #endregion Member Variables

        #region Constructor

        /// <summary>
        /// Factory method for creating an instance of ReflectedCommand
        /// which is optionally wrapped by ConfirmationCommand if it needs
        /// confirmation.
        /// </summary>
        /// <param name="methInfo">Reflection MethodInfo for the command method</param>
        /// <returns>command</returns>
        public static ICommand CreateInstance(MethodInfo methInfo)
        {
            ICommand cmd = new ReflectedCommand(methInfo);
            if (methInfo.IsDefined(typeof(ConfirmationAttribute), false))
            {
                ConfirmationAttribute confAttr = (ConfirmationAttribute)methInfo.GetCustomAttributes(typeof(ConfirmationAttribute), false)[0];
                ConfirmationCommand confCmd = new ConfirmationCommand(cmd, confAttr.Message, confAttr.CancellationMessage);
                cmd = confCmd;
            }
            return cmd;
        }

        /// <summary>
        /// Constructs an instace of the method helper
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="methInfo">Reflection methodInfo object</param>
        /// <param name="level">The minimum player level required to Execute the Command</param>
        /// <param name="description">A description of the method</param>
        private ReflectedCommand(MethodInfo methInfo)
        {
            _name = methInfo.Name;
            _methodInfo = methInfo;

            // set defaults
            this._level = 1;
            this._roles = new string[0];

            // Check for defaults at the class level
            if (methInfo.DeclaringType.IsDefined(typeof(CommandDefaultsAttribute), false))
            {
                CommandDefaultsAttribute defaults = (CommandDefaultsAttribute) methInfo.DeclaringType.GetCustomAttributes(typeof(CommandDefaultsAttribute), false)[0];
                if (defaults.Level != -1)
                    this._level = defaults.Level;

                if (defaults.Roles != null && defaults.Roles != "")
                    this._roles = defaults.Roles.Split(',', ' ');

                if (defaults.ClientTypes != null)
                    this._clientTypes = defaults.ClientTypes;
            }
            CommandAttribute cmdAttr = (CommandAttribute) methInfo.GetCustomAttributes(typeof(CommandAttribute), false)[0];

            if (cmdAttr.Level != -1)
                this._level = cmdAttr.Level;

            this._description = cmdAttr.Description;


            if (cmdAttr.Roles != null && cmdAttr.Roles != "")
                this._roles = cmdAttr.Roles.Split(',', ' ');

            if (cmdAttr.ClientTypes != null)
                this._clientTypes = cmdAttr.ClientTypes;

            this._aliases = cmdAttr.Aliases ?? new string[0];

            _argCount = 0;
            foreach (ParameterInfo param in methInfo.GetParameters())
            {
                if (param.IsDefined(typeof(ActorAttribute), false))
                {
                    //suppress count
                }
                else if (param.IsDefined(typeof(CustomParseAttribute), false))
                {
                    _argCount++;
                    _customParse = true;
                }
                else
                {
                    _argCount++;
                }
            }
        }

        #endregion Constructor

        #region Properties


        /// <summary>
        /// A description of what the Command does
        /// </summary>
        public string Description
        {
            get { return _description; }
        }


        /// <summary>
        /// Gets the System.Reflection.MethodInfo for this method
        /// </summary>
        public MethodInfo method
        {
            get { return _methodInfo; }
        }

        #endregion Properties

        #region Methods

        public override bool ConvertArguments(string invokedName, Living self, object[] arguments, out object[] convertedArguments, out Message errorMessage)
        {
            ParameterInfo[] parms = _methodInfo.GetParameters();
            convertedArguments = new object[parms.Length];

            int argIndex = 0;
            for (int i = 0; i < parms.Length; i++)
            {
                ParameterInfo param = parms[i];
                object arg;

                if (param.IsDefined(typeof(CustomParseAttribute), false))
                {
                    convertedArguments[i] = arguments[argIndex++];
                }
                else if (param.IsDefined(typeof(ActorAttribute), false))
                {
                    // most of the time players are executing the command,
                    // but sometimes it might be a mobile, make sure the command can
                    // accept the type of actor
                    if (param.ParameterType.IsAssignableFrom(self.GetType()))
                    {
                        convertedArguments[i] = self;
                    }
                    else
                    {
                        errorMessage = new ErrorResourceMessage("InvalidActor");
                        ((ErrorResourceMessage)errorMessage).Parameters["ActorType"] = self.GetType().Name;
                        convertedArguments = null;
                        return false;
                    }                    
                }
                else if (param.IsDefined(typeof(LookupAttribute), false))
                {
                    object target = arguments[argIndex++];
                    object result = null;
                    LookupAttribute attr = (LookupAttribute)param.GetCustomAttributes(typeof(LookupAttribute), false)[0]; 
                    if (param.GetType().IsInstanceOfType(target))
                    {
                        result = target;
                    }
                    else if (target is string)
                    {                        
                        ObjectQuery query = attr.ConstructQuery((string) target);
                        result = QueryManager.GetInstance().Find(self, query);
                    }
                    if (result == null && attr.IsRequired)
                    {
                        errorMessage = new ErrorResourceMessage("NotHere");
                        ((ErrorResourceMessage)errorMessage).Parameters["target"] = target;
                        convertedArguments = null;
                        return false;
                    }
                    else
                    {
                        convertedArguments[i] = result;
                    }
                }
                else
                {
                    try
                    {
                        arg = Convert.ChangeType(arguments[argIndex++], param.ParameterType);
                        convertedArguments[i] = arg;
                    }
                    catch (FormatException e)
                    {
                        errorMessage = new StringMessage(MessageType.PlayerError, invokedName, "Wrong number or type of arguments to " + invokedName + "\r\n");
                        convertedArguments = null;
                        return false;
                    }
                }
            }
            errorMessage = null;
            return true;            
        }

        public override Message Invoke(string invokedName, Living actor, object[] arguments)
        {
            try
            {
                object result = _methodInfo.Invoke(actor, arguments);
                if (result is Message)
                {
                    return (Message)result;
                }
                else if (result != null)
                {
                    return new StringMessage(MessageType.Information, invokedName, result.ToString());
                }
                else
                {
                    return null;
                }
            } catch (Exception e) {
                string error = "Error during execution of command '{0}' arguments: [";
                foreach(object o in arguments) {
                    if (o == null)
                        error += "null";
                    else
                        error += o.ToString();
                    error += ",";
                }
                if (arguments.Length > 0)
                    error = error.Substring(0, error.Length - 1);
                error += "]";
                error += " invoked by {1}.";
                logger.Error(string.Format(error, invokedName, actor.Title), e);
                // send generic message to player
                ErrorResourceMessage msg = new ErrorResourceMessage(MessageType.SystemError, Namespaces.SystemError, "SystemError");
                return msg;
            }
        }

        #endregion Methods
    }
}
