using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Core.Command.ArgumentConversion
{

    public abstract class CustomAttributeConverter
    {
        public abstract Type AttributeType { get; }
        public abstract object Convert(Argument argument, ArgumentConversionContext context);
    }

    public abstract class CustomAttributeConverter<TAttribute> : CustomAttributeConverter where TAttribute : CommandArgumentAttribute
    {
        public override Type AttributeType
        {
            get
            {
                return typeof(TAttribute);
            }
        }
    }
}
