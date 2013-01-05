using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using System.Configuration;

namespace Mirage.Core
{
    public static class WindsorExtensions
    {
        /// <summary>
        /// Supplies a dependency on an AppSetting from the config file
        /// </summary>
        /// <typeparam name="TComponent">the component type</typeparam>
        /// <param name="registration">component registration</param>
        /// <param name="dependencyType">the type of the dependency</param>
        /// <param name="appSettingName">the name of the app setting</param>
        /// <param name="dependencyName">the dependency name, if null will use the app setting name</param>
        /// <returns>component registration</returns>
        public static ComponentRegistration<TComponent> DependsOnAppSetting<TComponent>(this ComponentRegistration<TComponent> registration, Type dependencyType, string appSettingName, string dependencyName = null) where TComponent : class
        {
            return registration.DependsOn(Property.ForKey(dependencyName ?? appSettingName).Eq(Convert.ChangeType(ConfigurationManager.AppSettings[appSettingName], dependencyType)));
        }

        /// <summary>
        /// Supplies an optional dependency on an AppSetting from the config file.  If the appSetting
        /// is not present the dependency will not be added
        /// </summary>
        /// <typeparam name="TComponent">the component type</typeparam>
        /// <param name="registration">component registration</param>
        /// <param name="dependencyType">the type of the dependency</param>
        /// <param name="appSettingName">the name of the app setting</param>
        /// <param name="dependencyName">the dependency name, if null will use the app setting name</param>
        /// <returns>component registration</returns>
        public static ComponentRegistration<TComponent> OptionallyDependsOnAppSetting<TComponent>(this ComponentRegistration<TComponent> registration, Type dependencyType, string appSettingName, string dependencyName = null) where TComponent : class
        {
            if (ConfigurationManager.AppSettings[appSettingName] != null)
            {
                return registration.DependsOn(Property.ForKey(dependencyName ?? appSettingName).Eq(Convert.ChangeType(ConfigurationManager.AppSettings[appSettingName], dependencyType)));
            }
            return registration;
        }
    }
}
