using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mirage.Core;
using Mirage.Game.World;
using Mirage.Core.Messaging;

namespace Mirage.Game.Communication
{
    public class MessageFormatter
    {
        IViewManager viewManager;

        public const string AddNewLine = "AddNewLine";
        static MessageFormatter()
        {
            Instance = new MessageFormatter();
        }

        public MessageFormatter(IViewManager viewManager)
        {
            this.viewManager = viewManager;
        }

        public MessageFormatter()
        {
            this.viewManager = new ViewManager();
        }

        public static MessageFormatter Instance { get; set; }

        public StringMessage Format(IReceiveMessages recipient, object actor, MessageDefinition messageDefinition, object target = null, object anonymousTypeOrDictArgs = null)
        {
            var msg = Format(recipient, actor, messageDefinition.Name, messageDefinition.Text, target, anonymousTypeOrDictArgs);
            if (messageDefinition.MessageType != MessageType.Unknown)
                msg.MessageType = messageDefinition.MessageType;
            return msg;
        }

        public StringMessage Format(IReceiveMessages recipient, object actor, string messageID, string formatSpec, object target = null, object anonymousTypeOrDictArgs = null)
        {
            StringMessage result = new StringMessage();
            bool messageInvalid = false;
            IDictionary<string, object> args = null;
            if (anonymousTypeOrDictArgs is IDictionary<string, object>) {
                args = (IDictionary<string, object>) anonymousTypeOrDictArgs;
            }
            else if (anonymousTypeOrDictArgs != null)
            {
                args = ReflectionUtils.ObjectToDictionary(anonymousTypeOrDictArgs);
            }
            else
            {
                args = new Dictionary<string, object>(0);
            }

            result.Text = Regex.Replace(formatSpec, @"\$\{([^}]+)\}",
                (match) =>
                {
                    if (messageInvalid)
                        return "";

                    string spec = match.Groups[1].Value;
                    string[] specParts = spec.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (specParts.Length == 0)
                        return spec;
                    string formatObjName = specParts[0];
                    string formatArg = specParts.Length > 1 ? specParts[1] : "";

                    object specTarget = null;
                    switch (formatObjName.ToLower())
                    {
                        case "actor":
                            specTarget = actor;
                            break;
                        case "target":
                            specTarget = target;
                            break;
                        case "object":
                            if (!args.TryGetValue("object", out specTarget))
                                args.TryGetValue("object0", out specTarget);
                            break;
                        default:
                            args.TryGetValue(formatObjName.ToLower(), out specTarget);
                            break;
                    }
                    if (specTarget == null)
                    {
                        messageInvalid = true;
                        return "";
                    }
                    string value = "";
                    switch (formatArg.ToLower())
                    {
                        case "name":
                        case "title":
                        case "":
                            if (specTarget is IViewable && recipient is Living)
                                value = viewManager.GetTitle((Living)recipient, (IViewable)specTarget);
                            else
                                value = specTarget.ToString();
                            break;
                        case "short":
                            if (specTarget is IViewable && recipient is Living)
                                value = viewManager.GetShort((Living)recipient, (IViewable)specTarget);
                            else
                                value = specTarget.ToString();
                            break;
                        case "he":
                        case "she":
                            value = GetGenderString(specTarget, "he", "she", "it");
                            break;
                        case "him":
                            value = GetGenderString(specTarget, "him", "her", "it");
                            break;
                        case "his":
                            value = GetGenderString(specTarget, "his", "her", "its");
                            break;
                        default:
                            value = spec;
                            break;
                    }
                    result[spec] = value;
                    return value;
                });
            if (messageInvalid)
                return null;

            result.Text = result.Text.ToUpperFirst();
            bool doAddNewLine = true;
            object addNewlineprop = null;
            args.TryGetValue(AddNewLine, out addNewlineprop);
            if (addNewlineprop != null)
            {
                doAddNewLine = Convert.ToBoolean(addNewlineprop);
            }
            if (doAddNewLine)
            {
                if (!result.Text.EndsWith(Environment.NewLine))
                    result.Text += Environment.NewLine;
            }
            result.MessageType = MessageType.Information;
            result.Name = new MessageName(messageID);
            if (result.Name.FullName.Contains("error"))
                result.MessageType = MessageType.PlayerError;
            else
                result.MessageType = MessageType.Information;
            return result;
        }

        private string GetGenderString(object target, params string[] strings)
        {
            Living living = target as Living;
            if (living == null || living.Gender == GenderType.Other)
                return strings[2];
            else if (living.Gender == GenderType.Male)
                return strings[0];
            else if (living.Gender == GenderType.Female)
                return strings[1];
            else
                return strings[2];
        }

    }
}
