using System;
using System.Collections.Generic;
using System.Reflection;
using Mirage.Core.Collections;
using Mirage.Core.Messaging;
namespace Mirage.Core.Command
{
    /// <summary>
    /// Parses and Invokes commands from the players.  The commands are registered first by
    /// registering types that expose commands or command instances themselves
    /// </summary>
    public class CommandInvoker
    {
        private static CommandInvoker _instance = new CommandInvoker();

        private HashSet<Type> _registeredTypes;
        private IIndexedDictionary<ICommand> _methods;
        private HashSet<char> _singleCharCommands;

        private CommandInvoker()
        {
            _registeredTypes = new HashSet<Type>();
            _methods = new IndexedDictionary<ICommand>();
            _singleCharCommands = new HashSet<char>();
        }

        /// <summary>
        /// Gets the static instance of the class instance.
        /// </summary>
        public static CommandInvoker Instance
        {
            get { return _instance; }
        }


        /// <summary>
        /// Registers a command with the system
        /// </summary>
        /// <param name="command">the command to register</param>
        public void RegisterCommand(ICommand command)
        {
            if (command.Aliases != null && command.Aliases.Length > 0)
            {
                foreach (string alias in command.Aliases)
                {
                    _methods.Put(alias, command);
                    if (alias.Length == 1 && !char.IsLetterOrDigit(alias[0]))
                        _singleCharCommands.Add(alias[0]);
                }
            }
            else
            {
                _methods.Put(command.Name, command);
                if (command.Name.Length == 1 && !char.IsLetterOrDigit(command.Name[0]))
                    _singleCharCommands.Add(command.Name[0]);
            }
        }

        /// <summary>
        /// Register a class that exposes Command methods.
        /// The methods must be marked with the CommandAttribute
        /// </summary>
        /// <param name="objectType">Type of the object containing command methods</param>
        /// <param name="commandFactory">The command factory for creating commands from the methods.</param>
        /// <see cref="Mirage.Core.Command.CommandAttribute"/>
        public void RegisterTypeMethods(Type objectType, IReflectedCommandFactory commandFactory)
        {
            if (_registeredTypes.Contains(objectType))
            {
                return;
            }
            IReflectedCommandGroup group = commandFactory.GetCommandGroup(objectType);

            foreach (MethodInfo mInfo in objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static /* | BindingFlags.DeclaredOnly*/))
            {
                if (mInfo.IsDefined(typeof(CommandAttribute), false))
                {
                    ICommand command = commandFactory.CreateCommand(mInfo, group);
                    RegisterCommand(command);
                }
            }
            _registeredTypes.Add(objectType);
        }

        /// <summary>
        /// parse the command from the actor and invoke it if it's a valid command
        /// </summary>
        /// <param name="actor">the actor</param>
        /// <param name="commandString">the command and arguments</param>
        /// <returns>true if invoked successfully</returns>
        public bool Interpret(IActor actor, string commandString)
        {
            ArgumentParser parser;
            string commandName;
            string args;

            if (commandString != null && commandString.Length > 0 && _singleCharCommands.Contains(commandString[0]))
            {
                commandName = commandString.Substring(0, 1);
                args = commandString.Substring(1);
            }
            else
            {
                parser = new ArgumentParser(commandString);
                commandName = parser.GetNextArgument();
                args = parser.GetRest().TrimStart(null);
            }

            IEnumerable<ICommand> methods = GetAvailableCommands(commandName);
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

        /// <summary>
        /// Invokes a command with provided arguments
        /// </summary>
        /// <param name="actor">the actor</param>
        /// <param name="commandName">the name of the command to invoke</param>
        /// <param name="arguments">the arguments for the command</param>
        /// <returns>true if a command was invoked successfully</returns>
        public bool Interpret(IActor actor, string commandName, object[] arguments)
        {

            IEnumerable<ICommand> methods = GetAvailableCommands(commandName);
            bool fCommandInvoked = false;
            List<CanidateCommand> canidateCommands = new List<CanidateCommand>();

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
        public IEnumerable<ICommand> GetAvailableCommands(string commandName)
        {
            return _methods.FindStartsWith(commandName);
        }

        /// <summary>
        /// Returns the list of all available commands
        /// </summary>
        /// <returns>list of commands</returns>
        public IEnumerable<ICommand> GetAvailableCommands()
        {
            return _methods;
        }

        private class CanidateCommand : IComparable<CanidateCommand>
        {
            public CanidateCommand(ICommand command, string invokedName)
            {
                this.Command = command;
                this.InvokedName = invokedName;
            }

            public ICommand Command { get; set; }

            public string InvokedName { get; set; }

            public object[] Arguments { get; set; }

            public IMessage ErrorMessage { get; set; }

            public bool Validated { get; set; }

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