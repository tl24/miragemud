using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;

namespace Mirage.Game
{
    /// <summary>
    /// WindsorContainer converter class used to convert configuration nodes into ServiceEntryConverter
    /// Node should look like 
    /// <![CDATA[
    /// <serviceentry><service>${someservice}</service><method>Execute</method></serviceentry>
    /// ]]>
    /// </summary>
    public class ServiceEntryConverter : AbstractTypeConverter
    {
        public ServiceEntryConverter()
        {
        }

        public override bool CanHandleType(Type type)
        {
            return type == typeof(ServiceEntry);
        }

        public override object PerformConversion(String value, Type targetType)
        {
            throw new NotImplementedException();
        }

        public override object PerformConversion(IConfiguration configuration, Type targetType)
        {
            IServiceExecutor service = null;
            string key = null;

            foreach (IConfiguration childConfig in configuration.Children)
            {
                if (childConfig.Name == "method")
                {
                    key = (String)
                        Context.Composition.PerformConversion(childConfig, typeof(String));
                }
                else if (childConfig.Name == "service")
                {
                    service = (IServiceExecutor)
                        Context.Composition.PerformConversion(childConfig, typeof(IServiceExecutor));
                }                
            }
            return new ServiceEntry(service, key);
        }
    }
}
