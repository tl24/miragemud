using System;
using System.Reflection;
using log4net;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Query;
using System.Collections;
using Mirage.Core.Messaging;


namespace Mirage.Game.Command.Infrastructure
{
    /// <summary>
    /// Helper class for holding Attributes about a Command and invoking it.
    /// </summary>
    public class ReflectedCommand : CommandBase, ICommand
    {
        private static ILog logger = LogManager.GetLogger(typeof(ReflectedCommand));

        #region Member Variables

        private ArgumentList _arguments;
        private ReflectedCommandGroup _group;

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
            Name = methInfo.Name;
            Method = methInfo;

            // set defaults
            Level = 1;
            Roles = new string[0];

            if (!Method.IsStatic)
            {
                // only use the group if this method is an instance method
                this._group = group;
            }
            // Check for defaults at the class level
            if (methInfo.DeclaringType.IsDefined(typeof(CommandDefaultsAttribute), false))
            {
                CommandDefaultsAttribute defaults = (CommandDefaultsAttribute) methInfo.DeclaringType.GetCustomAttributes(typeof(CommandDefaultsAttribute), false)[0];
                if (defaults.Level != -1)
                    Level = defaults.Level;

                if (defaults.Roles != null && defaults.Roles != "")
                    Roles = defaults.Roles.Split(',', ' ');

                if (defaults.ClientTypes != null)
                    ClientTypes = defaults.ClientTypes;
            }
            CommandAttribute cmdAttr = (CommandAttribute) methInfo.GetCustomAttributes(typeof(CommandAttribute), false)[0];

            if (cmdAttr.Level != -1)
                Level = cmdAttr.Level;

            this.Description = cmdAttr.Description;


            if (cmdAttr.Roles != null && cmdAttr.Roles != "")
                Roles = cmdAttr.Roles.Split(',', ' ');

            if (cmdAttr.ClientTypes != null)
                ClientTypes = cmdAttr.ClientTypes;

            if (cmdAttr.Priority != 0)
                Priority = cmdAttr.Priority;

            Aliases = cmdAttr.Aliases ?? new string[0];

            ArgCount = 0;
            foreach (ParameterInfo param in methInfo.GetParameters())
            {
                if (param.IsDefined(typeof(ActorAttribute), false))
                {
                    //suppress count
                }
                else if (param.IsDefined(typeof(CustomParseAttribute), false))
                {
                    ArgCount++;
                    CustomParse = true;
                }
                else
                {
                    ArgCount++;
                }
            }
        }

        #endregion Constructor

        #region Properties


        /// <summary>
        /// A description of what the Command does
        /// </summary>
        public string Description { get; private set; }


        /// <summary>
        /// Gets the System.Reflection.MethodInfo for this method
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Returns the expected arguments for this command's method
        /// </summary>
        protected ArgumentList Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    _arguments = new ArgumentList(Method.GetParameters());
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
            if (_group != null)
            {
                //the group is the class that contains this method
                //if it implements ICommandGroup then give it a chance to define
                // custom conversion handlers
                ICommandGroup commandGroup = _group.GetInstance() as ICommandGroup;
                if (commandGroup != null)
                    commandGroup.InitializeArgumentHandlers(arguments);
            }
            foreach (Argument argument in arguments)
            {
                if (argument.Handler != null)
                    continue;

                if (argument.Parameter.IsDefined(typeof(CustomParseAttribute), false))
                    argument.Handler = ConvertCustomParse;
                else if (argument.Parameter.IsDefined(typeof(ActorAttribute), false))
                    argument.Handler = ConvertActor;
                else if (argument.Parameter.IsDefined(typeof(LookupAttribute), false))
                    argument.Handler = ConvertLookupArgument;
                else if (argument.Parameter.IsDefined(typeof(ConstAttribute), true))
                    argument.Handler = ConvertConstArgument;
                else
                    argument.Handler = DefaultConverter;
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
                context.ErrorMessage = MessageFormatter.Instance.Format(context.Actor, context.Actor, CommonMessages.ErrorInvalidActor, null, new { actorType = context.Actor.GetType().Name });
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
                var collection = MudFactory.GetObject<MudWorld>().ResolveUri(context.Actor, attr.BaseUri) as IEnumerable;
                if (collection != null)
                    result = collection.FindOne((string)target, attr.MatchType);
            }
            if (result == null && attr.IsRequired)
            {
                //ResourceMessage errorMessage = (ResourceMessage) MudFactory.GetObject<IMessageFactory>().GetMessage("common.error.NotHere");
                //errorMessage["target"] = target;
                var errorMessage = MessageFormatter.Instance.Format(context.Actor as Living, context.Actor, CommonMessages.ErrorNotHere, target);
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
            catch (FormatException)
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
                if (Method.DeclaringType == actor.GetType())
                    instance = actor;
                else if (_group != null)
                    instance = _group.GetInstance();

                object result = Method.Invoke(instance, arguments);
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
                return ve.CreateMessage(actor);            
            } catch (Exception e) {
                if (e.InnerException is ValidationException)
                {
                    return ((ValidationException)e.InnerException).CreateMessage(actor);
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
                    return MessageFormatter.Instance.Format(actor, actor, CommonMessages.ErrorSystem);
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
            ParameterInfo[] parms = Method.GetParameters();
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
