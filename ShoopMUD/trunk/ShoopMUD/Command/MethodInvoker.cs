using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Shoop.Data;
using Shoop.IO;
using System.Reflection;

using Shoop.Util;
using Shoop.Communication;
namespace Shoop.Command
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
        /// <see cref="rom.attribute.CommandAttribute"/>
        /// <param name="t">The type to register</param>
        public static void registerType(Type t)
        {
            if (!registeredTypes.ContainsKey(t))
            {
                getTypeMethods(t);
                registeredTypes[t] = true;
            }
        }

        private static void getTypeMethods(Type objectType)
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
                            methods.put(alias, command);
                        }
                    }
                    else
                    {
                        methods.put(command.Name, command);
                    }
                }
            }
        }

        public static bool interpret(Player player, string command)
        {
            ArgumentParser parser;
            string commandName;
            string args;

            //TODO: Come up with a better method for single character commands
            if (command.StartsWith("'"))
            {
                commandName = "'";
                args = command.Substring(1);
            }
            else
            {
                parser = new ArgumentParser(command);
                commandName = parser.getNextArgument();
                args = parser.getRest().TrimStart(null);
            }

            IList<ICommand> methods = getAvailableMethods(commandName);
            bool fCommandFound = false;
            bool fCommandInvoked = false;
            List<CanidateCommand> canidateCommands = new List<CanidateCommand>();
            
            bool fCommandValidated = true;

            foreach (ICommand method in methods)
            {
                if (method.Level <= player.Level)
                {
                    fCommandFound = true;
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
                    CanidateCommand canidate = new CanidateCommand(method, commandName, commandArgs);
                    canidateCommands.Add(canidate);
                    object context;
                    Message errorMessage;
                    canidate.Validated = method.ValidateTypes(canidate.InvokedName, player, canidate.StringArgs, out context, out errorMessage);
                    canidate.Context = context;
                    canidate.ErrorMessage = errorMessage;
                    if (canidate.Validated)
                    {
                        fCommandValidated = true;
                    }
                }
            }

            if (canidateCommands.Count > 0)
            {
                canidateCommands.Sort();
                CanidateCommand first = canidateCommands[0];
                if (first.Validated)
                {
                    Message result = first.Command.Invoke(first.InvokedName, player, first.StringArgs, first.Context);
                    fCommandInvoked = true;
                    if (result != null)
                        player.Write(result);
                }
                else
                {
                    player.Write(first.ErrorMessage);
                }
            }
            else
            {
                player.Write(new StringMessage(MessageType.PlayerError, "NoCommandFound", "Huh?\r\n"));
            }
            return fCommandInvoked;
        }

        private static IList<ICommand> getAvailableMethods(string commandName)
        {
            return methods.findStartsWith(commandName);
        }

        private class CanidateCommand : IComparable<CanidateCommand>
        {
            private ICommand _command;
            private string _invokedName;
            private string[] _stringArgs;
            private object context;
            private Message _errorMessage;
            private bool _validated;

            public CanidateCommand(ICommand command, string invokedName, string[] arguments)
            {
                this._command = command;
                this._invokedName = invokedName;
                this._stringArgs = arguments;
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

            public string[] StringArgs
            {
                get { return this._stringArgs; }
                set { this._stringArgs = value; }
            }

            public object Context
            {
                get { return this.context; }
                set { this.context = value; }
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


            #region IComparable<CanidateCommand> Members

            public int CompareTo(CanidateCommand other)
            {
                if (other.Validated && !this.Validated)
                    return -1;
                if (this.Validated && !other.Validated)
                    return 1;
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