using System;
using System.Collections.Generic;
using System.Reflection;
using Mirage.Core.Collections;
using Mirage.Game.Communication;
using Mirage.Game.World;
namespace Mirage.Game.Command
{
    public static class MethodInvoker
    {
        private static Dictionary<Type, bool> registeredTypes;
        private static IIndexedDictionary<ICommand> methods;

        static MethodInvoker()
        {
            registeredTypes = new Dictionary<Type, bool>();
            methods = new IndexedDictionary<ICommand>();
        }

        /// <summary>
        ///     Register a class that exposes Command methods.
        ///     The methods must be marked with the CommandAttribute
        /// </summary>
        /// <see cref="Mirage.Core.Command.CommandAttribute"/>
        /// <param name="t">The type to register</param>
        public static void RegisterType(Type t)
        {
            if (!registeredTypes.ContainsKey(t))
            {
                GetTypeMethods(t);
                registeredTypes[t] = true;
            }
        }

        /// <summary>
        /// Registers a command with the system
        /// </summary>
        /// <param name="command">the command to register</param>
        public static void RegisterCommand(ICommand command)
        {
            if (command.Aliases != null && command.Aliases.Length > 0)
            {
                foreach (string alias in command.Aliases)
                {
                    methods.Put(alias, command);
                }
            }
            else
            {
                methods.Put(command.Name, command);
            }
        }

        private static void GetTypeMethods(Type objectType)
        {
            ReflectedCommandGroup group = new ReflectedCommandGroup(objectType);

            foreach (MethodInfo mInfo in objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static /* | BindingFlags.DeclaredOnly*/))
            {
                if (mInfo.IsDefined(typeof(CommandAttribute), false))
                {
                    ICommand command = ReflectedCommand.CreateInstance(mInfo, group);
                    RegisterCommand(command);
                }
            }
        }

        public static bool Interpret(IActor actor, string commandString)
        {
            ArgumentParser parser;
            string commandName;
            string args;

            //TODO: Come up with a better method for single character commands
            if (commandString.StartsWith("'"))
            {
                commandName = "'";
                args = commandString.Substring(1);
            }
            else
            {
                parser = new ArgumentParser(commandString);
                commandName = parser.GetNextArgument();
                args = parser.GetRest().TrimStart(null);
            }

            IList<ICommand> methods = GetAvailableCommands(commandName);
            bool fCommandInvoked = false;
            List<CanidateCommand> canidateCommands = new List<CanidateCommand>();


            foreach (ICommand method in methods)
            {
                if (method.CanInvoke(actor))
                {
                    parser = new ArgumentParser(args);
                    string[] commandArgs = new string[method.ArgCount];
                    int parsedArgs = 0;
                    for (parsedArgs = 0; parsedArgs < commandArgs.Length && !parser.IsEmpty(); parsedArgs++)
                    {
                        if (parsedArgs == commandArgs.Length - 1)
                        {
                            if (method.CustomParse)
                                commandArgs[parsedArgs] = parser.GetRest();
                            else
                                commandArgs[parsedArgs] = parser.GetNextArgument();
                        }
                        else
                        {
                            commandArgs[parsedArgs] = parser.GetNextArgument();
                        }
                    }

                    if (parsedArgs != method.ArgCount || !parser.IsEmpty())
                    {
                        continue;
                    }

                    //TODO: commandName is not really the invokedName...could just be a partial name
                    CanidateCommand canidate = new CanidateCommand(method, commandName);
                    canidateCommands.Add(canidate);
                    object[] convertedArguments;
                    IMessage errorMessage;
                    if (method.ConvertArguments(canidate.InvokedName, actor, commandArgs, out convertedArguments, out errorMessage))
                    {
                        canidate.Arguments = convertedArguments;
                        canidate.Validated = true;
                    }
                    else
                    {
                        canidate.ErrorMessage = errorMessage;
                    }
                }
            }

            if (canidateCommands.Count > 0)
            {
                canidateCommands.Sort();
                CanidateCommand first = canidateCommands[0];
                if (first.Validated)
                {
                    IMessage result = first.Command.Invoke(first.InvokedName, actor, first.Arguments);
                    fCommandInvoked = true;
                    if (result != null)
                        actor.Write(result);
                }
                else
                {
                    actor.Write(first.ErrorMessage);
                }
            }
            else
            {
                bool cmdFound = false;
                foreach (ICommand method in methods)
                {
                    if (method.CanInvoke(actor))
                    {
                        actor.Write(new StringMessage(MessageType.PlayerError, "usage", method.UsageHelp()));
                        cmdFound = true;
                    }
                }
                if (!cmdFound)
                    actor.Write(new StringMessage(MessageType.PlayerError, "NoCommandFound", "Huh?\r\n"));
            }
            return fCommandInvoked;
        }

        public static bool Interpret(IActor actor, string commandName, object[] arguments)
        {

            IList<ICommand> methods = GetAvailableCommands(commandName);
            bool fCommandInvoked = false;
            List<CanidateCommand> canidateCommands = new List<CanidateCommand>();

            Type clientType = null;
            if (actor is IPlayer && ((IPlayer)actor).Client != null)
            {
                clientType = ((IPlayer)actor).Client.GetType();
            }

            foreach (ICommand method in methods)
            {
                if (method.CanInvoke(actor))
                {
                    if (arguments.Length != method.ArgCount)
                    {
                        continue;
                    }

                    //TODO: commandName is not really the invokedName...could just be a partial name
                    CanidateCommand canidate = new CanidateCommand(method, commandName);
                    canidateCommands.Add(canidate);
                    object[] convertedArguments;
                    IMessage errorMessage;
                    if (method.ConvertArguments(canidate.InvokedName, actor, arguments, out convertedArguments, out errorMessage))
                    {
                        canidate.Arguments = convertedArguments;
                        canidate.Validated = true;
                    }
                    else
                    {
                        canidate.ErrorMessage = errorMessage;
                    }
                }
            }

            if (canidateCommands.Count > 0)
            {
                canidateCommands.Sort();
                CanidateCommand first = canidateCommands[0];
                if (first.Validated)
                {
                    IMessage result = first.Command.Invoke(first.InvokedName, actor, first.Arguments);
                    fCommandInvoked = true;
                    if (result != null)
                        actor.Write(result);
                }
                else
                {
                    actor.Write(first.ErrorMessage);
                }
            }
            else
            {
                actor.Write(new StringMessage(MessageType.PlayerError, "NoCommandFound", "Huh?\r\n"));
            }
            return fCommandInvoked;
        }

        /// <summary>
        /// Returns a list of available commands starting with the given string
        /// </summary>
        /// <param name="commandName">command string to search for</param>
        /// <returns>list of commands</returns>
        public static IList<ICommand> GetAvailableCommands(string commandName)
        {
            return methods.FindStartsWith(commandName);
        }

        /// <summary>
        /// Returns a list of available commands
        /// </summary>
        /// <param name="commandName">command string to search for</param>
        /// <returns>list of commands</returns>
        public static IList<ICommand> GetAvailableCommands()
        {
            return methods.GetAllValues();
        }

        private class CanidateCommand : IComparable<CanidateCommand>
        {
            private ICommand _command;
            private string _invokedName;
            private object[] _arguments;
            private IMessage _errorMessage;
            private bool _validated;

            public CanidateCommand(ICommand command, string invokedName)
            {
                this._command = command;
                this._invokedName = invokedName;
            }

            public ICommand Command
            {
                get { return this._command; }
                set { this._command = value; }
            }

            public string InvokedName
            {
                get { return this._invokedName; }
                set { this._invokedName = value; }
            }

            public object[] Arguments
            {
                get { return this._arguments; }
                set { this._arguments = value; }
            }

            public IMessage ErrorMessage
            {
                get { return this._errorMessage; }
                set { this._errorMessage = value; }
            }

            public bool Validated
            {
                get { return this._validated; }
                set { this._validated = value; }
            }

            public bool IsExactName
            {
                get
                {
                    if (this.Command.Aliases != null && this.Command.Aliases.Length > 0)
                    {
                        foreach (string alias in this.Command.Aliases)
                        {
                            if (alias.Equals(InvokedName, StringComparison.InvariantCultureIgnoreCase))
                                return true;
                        }
                    }
                    else
                    {
                        if (InvokedName.Equals(Command.Name, StringComparison.InvariantCultureIgnoreCase))
                            return true;
                    }
                    return false;
                }
            }
            #region IComparable<CanidateCommand> Members

            public int CompareTo(CanidateCommand other)
            {
                if (other.Validated && !this.Validated)
                    return 1;
                if (this.Validated && !other.Validated)
                    return -1;

                if (other.IsExactName && !this.IsExactName)
                    return 1;
                if (this.IsExactName && !other.IsExactName)
                    return -1;

                if (other.Command.CustomParse && !this.Command.CustomParse)
                    return -1;
                if (this.Command.CustomParse && !other.Command.CustomParse)
                    return 1;

                if (this.Command.Priority != other.Command.Priority)
                    return other.Command.Priority - this.Command.Priority;
                else
                    return other.Command.Level - this.Command.Level;
            }

            #endregion
        }
    }



}