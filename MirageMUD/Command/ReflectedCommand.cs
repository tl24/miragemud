using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mirage.Data;
using Mirage.Communication;
using Mirage.Data.Query;


namespace Mirage.Command
{
    /// <summary>
    /// Helper class for holding Attributes about a Command and invoking it.
    /// </summary>
    public class ReflectedCommand : CommandBase, ICommand
    {
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

            CommandAttribute cmdAttr = (CommandAttribute) methInfo.GetCustomAttributes(typeof(CommandAttribute), false)[0];

            this._level = cmdAttr.Level;
            this._description = cmdAttr.Description;
            this._roles = cmdAttr.Roles ?? new string[0];
            this._aliases = cmdAttr.Aliases ?? new string[0];
            this._clientTypes = cmdAttr.ClientTypes;

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
                        errorMessage = new ErrorResourceMessage("Error.InvalidActor", "Error.InvalidActor");
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
                        errorMessage = new ErrorResourceMessage("Error.NotHere", "Error.NotHere");
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
            object result = _methodInfo.Invoke(actor, arguments);
            if (result is Message)
            {
                return (Message) result;
            }
            else if (result != null)
            {
                return new StringMessage(MessageType.Information, invokedName, result.ToString());
            }
            else
            {
                return null;
            }
        }

        #endregion Methods
    }
}
