using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirage.Game.World;
using System.Collections;
using Mirage.Game.Communication;
using Mirage.Game.World.Query;
using Mirage.Core.Command.ArgumentConversion;

namespace Mirage.Game.Command.ArgumentConversion
{
    public class LookupAttributeConverter : CustomAttributeConverter<LookupAttribute>
    {
        public LookupAttributeConverter(MudWorld world)
        {
            this.World = world;
        }

        public MudWorld World { get; private set; }

        public override object Convert(Argument argument, ArgumentConversionContext context)
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
                var collection = World.ResolveUri(context.Actor, attr.BaseUri) as IEnumerable;
                if (collection != null)
                    result = collection.FindOne((string)target, attr.MatchType);
            }
            if (result == null && attr.IsRequired)
            {
                var errorMessage = MessageFormatter.Instance.Format(context.Actor as Living, context.Actor, CommonMessages.ErrorNotHere, target);
                context.ErrorMessage = errorMessage;
                // return null below
            }
            return result;
        }
    }
}
