using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Mirage.Data;
using Mirage.IO;
using System.Reflection;

using Mirage.Util;
using Mirage.Communication;
namespace Mirage.Command
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
        /// <see cref="Mirage.Command.CommandAttribute"/>
        /// <param name="t">The type to register</param>
        public static void RegisterType(Type t)
        {
            if (!registeredTypes.ContainsKey(t))
            {
                GetTypeMethods(t);
                registeredTypes[t] = true;
            }
        }

        private static void GetTypeMethods(Type objectType)
        {
            foreach (MethodInfo mInfo in objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static /* | BindingFlags.DeclaredOnly*/))
            {
                if (mInfo.IsDefined(typeof(CommandAttribute), false))
                {
                    ICommand command = ReflectedCommand.CreateInstance(mInfo);
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
            }
        }

        public static bool Interpret(Living actor, string commandString)
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
                commandName = parser.getNextArgument();
                args = parser.getRest().TrimStart(null);
            }

            IList<ICommand> methods = GetAvailableMethods(commandName);
            bool fCommandInvoked = false;
            List<CanidateCommand> canidateCommands = new List<CanidateCommand>();
            
            Type clientType = null;
            if (actor is Player && ((Player)actor).Client != null)
            {
                clientType = ((Player)actor).Client.GetType();
            }

            foreach (ICommand method in methods)
            {
                if (method.Level <= actor.Level && CheckClientType(clientType, method.ClientTypes))
                {
                    parser = new ArgumentParser(args);
                    string[] commandArgs = new string[method.ArgCount];
                    int parsedArgs = 0;
                    for (parsedArgs = 0; parsedArgs < commandArgs.Length && !parser.isEmpty(); parsedArgs++)
                    {
                        if (parsedArgs == commandArgs.Length - 1)
                        {
                            if (method.CustomParse)
                            {
                                commandArgs[parsedArgs] = parser.getRest();
                            }
                            else
                            {
                                commandArgs[parsedArgs] = parser.getNextArgument();
                            }
                        }
                        else
                        {
                            commandArgs[parsedArgs] = parser.getNextArgument();
                        }
                    }

                    if (parsedArgs != method.ArgCount || !parser.isEmpty())
                    {
                        continue;
                    }
                    int argCount = 0;
                    bool nextMethod = false;

                    //TODO: commandName is not really the invokedName...could just be a partial name
                    CanidateCommand canidate = new CanidateCommand(method, commandName);
                    canidateCommands.Add(canidate);
                    object[] convertedArguments;
                    Message errorMessage;
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
                    Message result = first.Command.Invoke(first.InvokedName, actor, first.Arguments);
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

        public static bool Interpret(Living actor, string commandName, object[] arguments)
        {

            IList<ICommand> methods = GetAvailableMethods(commandName);
            bool fCommandInvoked = false;
            List<CanidateCommand> canidateCommands = new List<CanidateCommand>();

            Type clientType = null;
            if (actor is Player && ((Player)actor).Client != null)
            {
                clientType = ((Player)actor).Client.GetType();
            }

            foreach (ICommand method in methods)
            {
                if (method.Level <= actor.Level && CheckClientType(clientType, method.ClientTypes))
                {
                    if (arguments.Length != method.ArgCount)
                    {
                        continue;
                    }
                    int argCount = 0;
                    bool nextMethod = false;

                    //TODO: commandName is not really the invokedName...could just be a partial name
                    CanidateCommand canidate = new CanidateCommand(method, commandName);
                    canidateCommands.Add(canidate);
                    object[] convertedArguments;
                    Message errorMessage;
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
                    Message result = first.Command.Invoke(first.InvokedName, actor, first.Arguments);
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
        /// Checks to see if the type of the player's client is within the allowed list
        /// or if all clients are accepted
        /// </summary>
        /// <param name="clientType">the players client type</param>
        /// <param name="allowedTypes">the list of allowed types</param>
        /// <returns>true if allowed</returns>
        private static bool CheckClientType(Type clientType, Type[] allowedTypes)
        {
            if (allowedTypes == null || allowedTypes.Length == 0)
            {
                return true;
            }
            else
            {
                if (clientType == null)
                {
                    return false;
                }
                else
                {
                    foreach (Type t in allowedTypes)
                    {
                        if (t.IsAssignableFrom(clientType))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        private static IList<ICommand> GetAvailableMethods(string commandName)
        {
            return methods.FindStartsWith(commandName);
        }

        private class CanidateCommand : IComparable<CanidateCommand>
        {
            private ICommand _command;
            private string _invokedName;
            private object[] _arguments;
            private object context;
            private Message _errorMessage;
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

            public Message ErrorMessage
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