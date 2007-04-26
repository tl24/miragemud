using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Shoop.Data;
using Shoop.IO;
using System.Reflection;
using Shoop.Attributes;
using Shoop.Util;
namespace Shoop.Command
{
    public static class MethodInvoker
    {
        private static Dictionary<Type, bool> registeredTypes;
        private static IIndexedDictionary<MethodHelper> methods;

        static MethodInvoker()
        {
            registeredTypes = new Dictionary<Type, bool>();
            methods = new IndexedDictionary<MethodHelper>();
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
                foreach (Attribute attr in mInfo.GetCustomAttributes(typeof(CommandAttribute), true))
                {
                    CommandAttribute cmdAttr = (CommandAttribute)attr;

                    MethodHelper helper = new MethodHelper(mInfo.Name, mInfo, cmdAttr.Level, cmdAttr.Description);
                    methods.put(mInfo.Name, helper);
                    break;
                }
            }
        }

        public static bool interpret(Player player, string command)
        {
            ArgumentParser parser = new ArgumentParser(command);
            string commandName = parser.getNextArgument();
            string args = parser.getRest().TrimStart(null);
            IList<MethodHelper> methods = getAvailableMethods(commandName);
            string errorMessage = "Huh?";
            bool fCommandFound = false;
            bool fCommandInvoked = false;

            foreach (MethodHelper method in methods)
            {
                if (method.Level <= player.level)
                {
                    fCommandFound = true;
                    errorMessage = method.Description;

                    parser = new ArgumentParser(args);
                    ParameterInfo[] parms = method.method.GetParameters();
                    object[] methodArgs = new object[parms.Length];
                    int argCount = 0;
                    bool nextMethod = false;

                    foreach (ParameterInfo param in parms)
                    {
                        object arg;

                        ArgumentTypeAttribute[] attr = (ArgumentTypeAttribute[])param.GetCustomAttributes(typeof(ArgumentTypeAttribute), false);
                        if (attr.Length > 0)
                        {
                            try
                            {
                                arg = parseSpecialArg(attr[0], player, parser);
                            }
                            catch (Exception e)
                            {
                                nextMethod = true;
                                break;
                            }
                        }
                        else
                        {
                            try
                            {
                                arg = Convert.ChangeType(parser.getNextArgument(), param.ParameterType);
                            } catch (FormatException e) {
                                nextMethod = true;
                                break;
                            }
                        }
                        if (arg == null)
                        {
                            nextMethod = true;
                            break;
                        }
                        else
                        {
                            methodArgs[argCount++] = arg;
                        }
                    }

                    if (nextMethod)
                    {
                        continue;
                    }

                    if (argCount == methodArgs.Length && parser.isEmpty())
                    {
                        fCommandInvoked = true;
                        object result = null;
                        if (!handleConfirmation(player, method, methodArgs))
                        {
                            result = method.method.Invoke(player, methodArgs);
                        }
                        if (result != null)
                        {
                            player.Descriptor.writeToBuffer(result.ToString());
                        }
                        break;
                    }
                }
            }
            if (fCommandFound)
            {
                if (!fCommandInvoked)
                {
                    player.Descriptor.writeToBuffer("Wrong number or type of arguments to " + commandName + "\r\n");
                }
            }
            else
            {
                player.Descriptor.writeToBuffer(errorMessage + "\r\n");
            }
            return fCommandInvoked;
        }

        private static bool handleConfirmation(Player player, MethodHelper method, object[] methodArgs)
        {
            ConfirmationAttribute[] attr = (ConfirmationAttribute[])method.method.GetCustomAttributes(typeof(ConfirmationAttribute), false);
            if (attr.Length > 0)
            {
                ConfirmationInterpreter interp = new ConfirmationInterpreter(player, method.method, methodArgs);
                if (attr[0].Message != null)
                    interp.Message = attr[0].Message;
                if (attr[0].CancellationMessage != null)
                    interp.CancellationMessage = attr[0].CancellationMessage;


                interp.requestConfirmation();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static object parseSpecialArg(ArgumentTypeAttribute attribute, Player player, ArgumentParser parser)
        {
            switch (attribute.ArgType)
            {
                case ArgumentType.Self:
                    return player;
                case ArgumentType.ToEOL:
                    string rest = parser.getRest();
                    if (rest == null)
                    {
                        rest = "";
                    }
                    return rest;
                    break;
                default:
                    throw new ApplicationException("Unrecognized ArgType: " + attribute.ArgType);
            }
        }

        private static IList<MethodHelper> getAvailableMethods(string commandName)
        {
            return methods.findStartsWith(commandName);
        }

    }

    /// <summary>
    /// Helper class for holding Attributes about a Command and invoking it.
    /// </summary>
    public class MethodHelper
    {
        private string _name;
        private int _ArgCount;

        /// <summary>
        /// Number of arguments to the Command, not including private arguments such as "self"
        /// </summary>
        public int ArgCount
        {
            get { return _ArgCount; }
        }
        private bool _hasRest;

        /// <summary>
        /// True if the Command has a ToEOL argument, which means it has a variable number of arguments
        /// </summary>
        public bool HasRest
        {
            get { return _hasRest; }
        }

        /// <summary>
        /// The name of the method
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        private int _level;

        /// <summary>
        /// The required player level to execute the Command
        /// </summary>
        public int Level
        {
            get { return _level; }
        }
        private string _description;

        /// <summary>
        /// A description of what the Command does
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        private MethodInfo _methodInfo;

        /// <summary>
        /// Gets the System.Reflection.MethodInfo for this method
        /// </summary>
        public MethodInfo method
        {
            get { return _methodInfo; }                 
        }

        /// <summary>
        /// Constructs an instace of the method helper
        /// </summary>
        /// <param name="name">The name of the method</param>
        /// <param name="methInfo">Reflection methodInfo object</param>
        /// <param name="level">The minimum player level required to execute the Command</param>
        /// <param name="description">A description of the method</param>
        public MethodHelper(string name, MethodInfo methInfo, int level, string description)
        {
            _name = name;
            _methodInfo = methInfo;
            this._level = level;
            this._description = description;
            _ArgCount = 0;
            foreach (ParameterInfo param in methInfo.GetParameters())
            {
                foreach (Shoop.Attributes.ArgumentTypeAttribute attr in methInfo.GetCustomAttributes(typeof(Shoop.Attributes.ArgumentTypeAttribute), false))
                {
                    if (attr.ArgType != Shoop.Attributes.ArgumentType.Self)
                    {
                        _ArgCount++;
                    }
                    if (attr.ArgType == Shoop.Attributes.ArgumentType.ToEOL)
                    {
                        _hasRest = true;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Invoke the method with the given arguments
        /// </summary>
        /// <param name="target">The object instance on which to invoke the method</param>
        /// <param name="actor">The player or caller of the Command</param>
        /// <param name="arguments">Arguments to the Command</param>
        /// <returns>Returns the string representation of the result or null if no return</returns>
        public string invoke(object target, Animate actor, string arguments)
        {
            object retVal = _methodInfo.Invoke(target, null);
            if (retVal != null)
            {
                return retVal.ToString();
            }
            else
            {
                return null;
            }
        }

    }

}