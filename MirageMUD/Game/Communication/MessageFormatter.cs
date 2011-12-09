using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mirage.Core.Collections;
using Mirage.Game.World;

namespace Mirage.Game.Communication
{
    public class MessageFormatter
    {
        IViewManager viewManager;

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

        public StringMessage Format(Living recipient, Living actor, string messageID, string formatSpec)
        {
            return Format(recipient, actor, messageID, formatSpec, null);
        }

        public StringMessage Format(Living recipient, Living actor, string messageID, string formatSpec, Living target)
        {
            return Format(recipient, actor, messageID, formatSpec, target, null);
        }

        public StringMessage Format(Living recipient, Living actor, string messageID, string formatSpec, Living target, IDictionary<string, object> args)
        {
            StringMessage result = new StringMessage();
            bool messageInvalid = false;
            args = args ?? new Dictionary<string, object>(0);

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
                            if (specTarget is IViewable)
                                value = viewManager.GetTitle(recipient, (IViewable)specTarget);
                            else
                                value = specTarget.ToString();
                            break;
                        case "short":
                            if (specTarget is IViewable)
                                value = viewManager.GetShort(recipient, (IViewable)specTarget);
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
            if (!result.Text.EndsWith(Environment.NewLine))
                result.Text += Environment.NewLine;

            result.MessageType = MessageType.Information;
            result.Name = new MessageName(messageID);
            if (result.Name.Namespace.Contains("error"))
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
