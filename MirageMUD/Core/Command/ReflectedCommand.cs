using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mirage.Core.Data;
using Mirage.Core.Communication;
using Mirage.Core.Data.Query;
using log4net;


namespace Mirage.Core.Command
{

    public delegate object ConvertArgumentHandler(Argument argument, ArgumentConversionContext context);

    /// <summary>
    /// Helper class for holding Attributes about a Command and invoking it.
    /// </summary>
    public class ReflectedCommand : CommandBase, ICommand
    {
        private static ILog logger = LogManager.GetLogger(typeof(ReflectedCommand));

        #region Member Variables

        private string _description;
        private MethodInfo _methodInfo;
        private ArgumentList _arguments;
        private ReflectedCommandGroup group;

        #endregion Member Variables

        #region Constructor

        /// <summary>
        /// Factory method for creating an instance of ReflectedCommand
        /// which is optionally wrapped by ConfirmationCommand if it needs
        /// confirmation.
        /// </summary>
        /// <param name="methInfo">Reflection MethodInfo for the command method</param>
        /// <returns>command</returns>
        public static ICommand CreateInstance(MethodInfo methInfo, ReflectedCommandGroup commandGroup)
        {
            ICommand cmd = new ReflectedCommand(methInfo, commandGroup);
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
        private ReflectedCommand(MethodInfo methInfo, ReflectedCommandGroup group)
        {
            _name = methInfo.Name;
            _methodInfo = methInfo;

            // set defaults
            this._level = 1;
            this._roles = new string[0];

            if (!_methodInfo.IsStatic)
            {
                // only use the group if this method is an instance method
                this.group = group;
            }
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

        /// <summary>
        /// Returns the expected arguments for this command's method
        /// </summary>
        protected ArgumentList Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    _arguments = new ArgumentList(_methodInfo.GetParameters());
                    InitializeArgumentHandlers(_arguments);
                }
                return _arguments;
            }
        }
        #endregion Properties

        #region Methods

        public override bool ConvertArguments(string invokedName, IActor actor, object[] arguments, out object[] convertedArguments, out IMessage errorMessage)
        {
            ArgumentList expectedArgs = this.Arguments;

            convertedArguments = new object[expectedArgs.Count];
            ArgumentConversionContext context = new ArgumentConversionContext(invokedName, actor, arguments);

            int index = 0;
            foreach (Argument arg in expectedArgs)
            {

                convertedArguments[index++] = arg.Convert(context);
                if (context.ErrorMessage != null)
                {
                    errorMessage = context.ErrorMessage;
                    convertedArguments = null;
                    return false;
                }
            }
            // if we get here, conversion was successful
            errorMessage = null;
            return true;            
        }

        /// <summary>
        /// Finds a conversion handler for each argument in the argument list
        /// </summary>
        /// <param name="arguments"></param>
        protected void InitializeArgumentHandlers(ArgumentList arguments)
        {
            if (group != null)
            {
                //the group is the class that contains this method
                //if it implements ICommandGroup then give it a chance to define
                // custom conversion handlers
                ICommandGroup commandGroup = group.GetInstance() as ICommandGroup;
                if (commandGroup != null)
                    commandGroup.InitializeArgumentHandlers(arguments);
            }
            foreach (Argument argument in arguments)
            {
                if (argument.Handler != null)
                    continue;

                if (argument.Parameter.IsDefined(typeof(CustomParseAttribute), false))
                    argument.Handler = new ConvertArgumentHandler(ConvertCustomParse);
                else if (argument.Parameter.IsDefined(typeof(ActorAttribute), false))
                    argument.Handler = new ConvertArgumentHandler(ConvertActor);
                else if (argument.Parameter.IsDefined(typeof(LookupAttribute), false))
                    argument.Handler = new ConvertArgumentHandler(ConvertLookupArgument);
                else if (argument.Parameter.IsDefined(typeof(ConstAttribute), true))
                    argument.Handler = new ConvertArgumentHandler(ConvertConstArgument);
                else
                    argument.Handler = new ConvertArgumentHandler(DefaultConverter);
            }
        }

        /// <summary>
        /// Convert a custom parsed attribute
        /// </summary>
        /// <returns>true if converted</returns>
        protected virtual object ConvertCustomParse(Argument argument, ArgumentConversionContext context)
        {
            return context.GetCurrentAndIncrement();
        }

        /// <summary>
        /// Converts the actor argument
        /// </summary>
        /// <returns>actor</returns>
        protected virtual object ConvertActor(Argument argument, ArgumentConversionContext context)
        {
            // most of the time players are executing the command,
            // but sometimes it might be a mobile, make sure the command can
            // accept the type of actor
            if (argument.Parameter.ParameterType.IsAssignableFrom(context.Actor.GetType()))
            {
                return context.Actor;
            }
            else
            {
                ResourceMessage rmsg = (ResourceMessage)MessageFactory.GetMessage("msg:/common/error/InvalidActor");
                rmsg["ActorType"] = context.Actor.GetType().Name;
                context.ErrorMessage = rmsg;
                return null;
            }
        }

        /// <summary>
        /// Converts a lookup query argument by using the input to perform the query and returning the result
        /// </summary>
        /// <returns>the lookup result</returns>
        protected virtual object ConvertLookupArgument(Argument argument, ArgumentConversionContext context)
        {
            object target = context.GetCurrentAndIncrement();
            object result = null;
            LookupAttribute attr = (LookupAttribute)argument.Parameter.GetCustomAttributes(typeof(LookupAttribute), false)[0];
            if (argument.Parameter.GetType().IsInstanceOfType(target))
            {
                result = target;
            }
            else if (target is string)
            {
                ObjectQuery query = attr.ConstructQuery((string)target);
                result = MudFactory.GetObject<IQueryManager>().Find(context.Actor, query);
            }
            if (result == null && attr.IsRequired)
            {
                ResourceMessage errorMessage = (ResourceMessage) MessageFactory.GetMessage("msg:/common/error/NotHere");
                errorMessage["target"] = target;
                context.ErrorMessage = errorMessage;
                // return null below
            }
            return result;
        }

        /// <summary>
        /// Converts a constant argument by returning the constant
        /// </summary>
        /// <returns>the constant</returns>
        protected virtual object ConvertConstArgument(Argument argument, ArgumentConversionContext context)
        {
            object target = context.GetCurrentAndIncrement();
            object result = null;
            ConstAttribute attr = (ConstAttribute)argument.Parameter.GetCustomAttributes(typeof(ConstAttribute), true)[0];
            if (attr.Constant.Equals(target.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return attr.Constant;
            }
            else
            {
                context.ErrorMessage = new StringMessage(MessageType.PlayerError, "NoCommandFound", "Huh?\r\n");
                return null;
            }
        }

        /// <summary>
        /// Does default conversion which attempts to convert the argument to its expected type      
        /// </summary>
        /// <returns>converted argument</returns>
        protected virtual object DefaultConverter(Argument argument, ArgumentConversionContext context)
        {
            try
            {
                return Convert.ChangeType(context.GetCurrentAndIncrement(), argument.Parameter.ParameterType);
            }
            catch (FormatException e)
            {
                context.ErrorMessage = new StringMessage(MessageType.PlayerError, context.InvokedName, "Wrong number or type of arguments to " + context.InvokedName + "\r\n");
                return null;
            }
        }

        /// <summary>
        /// Invokes the underlying method backing this command with the specified arguments
        /// </summary>
        /// <param name="invokedName">the name or alias used to invoke this command</param>
        /// <param name="actor">the actor that invoked this command</param>
        /// <param name="arguments">the arguments to the command</param>
        /// <returns>a message if the command returned one or null otherwise</returns>
        public override IMessage Invoke(string invokedName, IActor actor, object[] arguments)
        {
            try
            {
                object instance = null;
                if (_methodInfo.DeclaringType == actor.GetType())
                    instance = actor;
                else if (group != null)
                    instance = group.GetInstance();

                object result = _methodInfo.Invoke(instance, arguments);
                if (result is IMessage)
                {
                    return (IMessage)result;
                }
                else if (result != null)
                {
                    return new StringMessage(MessageType.Information, invokedName, result.ToString());
                }
                else
                {
                    return null;
                }
            } catch (ValidationException ve) {
                return ve.MessageObject;            
            } catch (Exception e) {
                if (e.InnerException is ValidationException)
                {
                    return ((ValidationException)e.InnerException).MessageObject;
                }
                else
                {
                    string error = "Error during execution of command '{0}' arguments: [";
                    foreach (object o in arguments)
                    {
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
                    logger.Error(string.Format(error, invokedName, actor), e);
                    // send generic message to player
                    ErrorResourceMessage msg = new ErrorResourceMessage(MessageType.SystemError, Namespaces.SystemError, "SystemError");
                    return msg;
                }
            }
        }

        public override string UsageHelp()
        {
            string nameOrAlias = "";
            if (Aliases.Length > 0)
                nameOrAlias = Aliases[0];
            else
                nameOrAlias = Name;

            string args = "";
            ParameterInfo[] parms = _methodInfo.GetParameters();
            foreach (ParameterInfo info in parms)
            {
                if (info.IsDefined(typeof(ActorAttribute), false))
                    continue;
                if (args.Length > 0)
                    args += " ";
                args += info.Name;
            }
            return "Usage: " + nameOrAlias + " " + args + "\r\n";
        }

        public override string ShortHelp()
        {
            string nameOrAlias = "";
            if (Aliases.Length > 0)
                nameOrAlias = Aliases[0];
            else
                nameOrAlias = Name;

            string desc = Description ?? "";
            if (desc.IndexOfAny(new char[] { '\n', '\r' }) >= 0)
                desc = desc.Substring(0, desc.IndexOfAny(new char[] { '\n', '\r' }));

            return nameOrAlias + " " + desc;
        }
        #endregion Methods
    }
}
