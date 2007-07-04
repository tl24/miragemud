using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Shoop.Data;
using Shoop.Communication;
using Shoop.Data.Query;


namespace Shoop.Command
{
    /// <summary>
    /// Helper class for holding Attributes about a Command and invoking it.
    /// </summary>
    public class ReflectedCommand : ICommand
    {
        #region Member Variables

        private string _name;
        private int _argCount;
        private string[] _aliases;
        private string[] _roles;
        private int _priority = 50;
        private bool _customParse;
        private int _level;
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
        /// <param name="level">The minimum player level required to execute the Command</param>
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
        /// Number of arguments to the Command, not including private arguments such as "self"
        /// </summary>
        public int ArgCount
        {
            get { return _argCount; }
        }

        /// <summary>
        /// True if the Command has a ToEOL argument, which means it has a variable number of arguments
        /// </summary>
        public bool CustomParse
        {
            get { return _customParse; }
        }

        /// <summary>
        /// The name of the method
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The required player level to execute the Command
        /// </summary>
        public int Level
        {
            get { return _level; }
        }

        /// <summary>
        /// A description of what the Command does
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        public string[] Aliases
        {
            get { return _aliases; }
        }

        public string[] Roles
        {
            get { return _roles; }
        }

        public int Priority
        {
            get { return _priority; }
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

        public bool ValidateTypes(string invokedName, Player self, string[] arguments, out object context, out Message errorMessage)
        {
            ParameterInfo[] parms = _methodInfo.GetParameters();
            object[] typedArgs = new object[parms.Length];

            int argIndex = 0;
            for (int i = 0; i < parms.Length; i++)
            {
                ParameterInfo param = parms[i];
                object arg;

                if (param.IsDefined(typeof(CustomParseAttribute), false))
                {
                    typedArgs[i] = arguments[argIndex++];
                }
                else if (param.IsDefined(typeof(ActorAttribute), false))
                {
                    typedArgs[i] = self;
                }
                else if (param.IsDefined(typeof(LookupAttribute), false))
                {
                    string target = arguments[argIndex++];
                    LookupAttribute attr = (LookupAttribute) param.GetCustomAttributes(typeof(LookupAttribute), false)[0];
                    ObjectQuery query = attr.ConstructQuery(target);
                    object result = QueryManager.GetInstance().Find(self, query);
                    if (result == null && attr.IsRequired)
                    {
                        errorMessage = new ErrorResourceMessage("Error.NotHere", "Error.NotHere");
                        ((ErrorResourceMessage)errorMessage).Parameters["target"] = target;
                        context = null;
                        return false;
                    }
                    else
                    {
                        typedArgs[i] = result;
                    }
                }
                else
                {
                    try
                    {
                        arg = Convert.ChangeType(arguments[argIndex++], param.ParameterType);
                        typedArgs[i] = arg;
                    }
                    catch (FormatException e)
                    {
                        errorMessage = new StringMessage(MessageType.PlayerError, invokedName, "Wrong number or type of arguments to " + invokedName + "\r\n");
                        context = null;
                        return false;
                    }
                }
            }
            context = typedArgs;
            errorMessage = null;
            return true;            
        }

        public Message Invoke(string invokedName, Player self, string[] arguments, object context)
        {
            object result = _methodInfo.Invoke(self, (object[])context);
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
