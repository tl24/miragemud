using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Mirage.Core.Messaging;
using Mirage.Game.Command.Infrastructure.ArgumentConversion;
using Mirage.Game.Command.Infrastructure.Guards;
using Mirage.Game.Communication;
using Mirage.Game.World;
using Mirage.Game.World.Query;


namespace Mirage.Game.Command.Infrastructure
{
    /// <summary>
    /// Helper class for holding Attributes about a Command and invoking it.
    /// </summary>
    public class ReflectedCommand : CommandBase, ICommand
    {
        private static ILog logger = LogManager.GetLogger(typeof(ReflectedCommand));

        #region Member Variables

        private List<Argument> _arguments;
        private IReflectedCommandGroup _group;

        #endregion Member Variables

        #region Constructor

        static ReflectedCommand()
        {
            Converters = new Dictionary<Type, Func<Argument, ArgumentConversionContext, object>>();
            // initialize defaults
            Converters[typeof(CustomParseAttribute)] = ConvertCustomParse;
            Converters[typeof(ActorAttribute)] = ConvertActor;
            Converters[typeof(ConstAttribute)] = ConvertConstArgument;
        }

        /// <summary>
        /// Constructs an instance of the method helper
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="methInfo">Reflection methodInfo object</param>
        /// <param name="level">The minimum player level required to Execute the Command</param>
        /// <param name="description">A description of the method</param>
        public ReflectedCommand(MethodInfo methInfo, IReflectedCommandGroup group)
        {
            Name = methInfo.Name;
            Method = methInfo;

            // set defaults
            Level = 1;

            if (!Method.IsStatic)
            {
                // only use the group if this method is an instance method
                this._group = group;
            }
            // create a dictionary by type so that the method can override the default guards
            var guardMap = new Dictionary<Type, ICommandGuard>();
            // Check for defaults at the class level
            foreach (CommandRestrictionAttribute restriction in methInfo.DeclaringType.GetCustomAttributes(typeof(CommandRestrictionAttribute), true))
            {
                var guard = restriction.CreateGuard();
                guardMap[guard.GetType()] = guard;
            }

            foreach (CommandRestrictionAttribute restriction in methInfo.GetCustomAttributes(typeof(CommandRestrictionAttribute), true))
            {
                var guard = restriction.CreateGuard();
                guardMap[guard.GetType()] = guard;
            }
            // set the guards now
            Guards.AddRange(guardMap.Values);
            CommandAttribute cmdAttr = (CommandAttribute)methInfo.GetCustomAttributes(typeof(CommandAttribute), false)[0];

            if (guardMap.ContainsKey(typeof(LevelGuard)))
            {
                Level = ((LevelGuard)guardMap[typeof(LevelGuard)]).Level;
            }

            this.Description = cmdAttr.Description;

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

        public static Dictionary<Type, Func<Argument, ArgumentConversionContext, object>> Converters { get; private set; }

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
        protected List<Argument> Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    _arguments = Method.GetParameters().Select(p => new Argument(p)).ToList();
                    InitializeArgumentHandlers(_arguments);
                }
                return _arguments;
            }
        }
        #endregion Properties

        #region Methods

        public override bool ConvertArguments(string invokedName, IActor actor, object[] arguments, out object[] convertedArguments, out IMessage errorMessage)
        {
            List<Argument> expectedArgs = this.Arguments;

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
        protected void InitializeArgumentHandlers(List<Argument> arguments)
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
                var argAttr = argument.Parameter.GetCustomAttributes(typeof(CommandArgumentAttribute), false).FirstOrDefault();
                if (argAttr != null && Converters.ContainsKey(argAttr.GetType()))
                {
                    argument.Handler = Converters[argAttr.GetType()];
                }
                if (argument.Handler == null)
                {
                    // still null, assign the default
                    argument.Handler = DefaultConverter;
                }
            }
        }

        /// <summary>
        /// Convert a custom parsed attribute
        /// </summary>
        /// <returns>true if converted</returns>
        protected static object ConvertCustomParse(Argument argument, ArgumentConversionContext context)
        {
            return context.GetCurrentAndIncrement();
        }

        /// <summary>
        /// Converts the actor argument
        /// </summary>
        /// <returns>actor</returns>
        protected static object ConvertActor(Argument argument, ArgumentConversionContext context)
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
                context.ErrorMessage = new StringMessage("common.error.invalidactor", "This command can not be executed by a " + context.Actor.GetType().Name + ".\r\n");
                return null;
            }
        }

        /// <summary>
        /// Converts a constant argument by returning the constant
        /// </summary>
        /// <returns>the constant</returns>
        protected static object ConvertConstArgument(Argument argument, ArgumentConversionContext context)
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
                    return new StringMessage("common.error.system", "A system error has occurred executing your command.");
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
