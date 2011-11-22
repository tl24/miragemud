using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor.Configuration;
using Mirage.Core.IO;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel;
using Mirage.Core.Util;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace Mirage.Core.Data
{
    /// <summary>
    /// Abstraction class for creating and referencing types
    /// </summary>
    public class MudFactory
    {
        private static MyContainer _instance;

        static MudFactory()
        {
            _instance = new MyContainer(new XmlInterpreter());
            
        }

        public static TObjectInterface GetObject<TObjectInterface>()
        {
            return _instance.Resolve<TObjectInterface>();
        }

        public static TObjectInterface GetObject<TObjectInterface>(object argumentsAsAnonymousType)
        {
            return _instance.Resolve<TObjectInterface>(argumentsAsAnonymousType);
        }

        public static object GetObject(Type interfaceType)
        {
            return _instance.Resolve(interfaceType);
            
        }

        public static void RegisterService(Type classType)
        {
            _instance.AddComponent(classType.FullName, classType);
        }

        private class MyContainer : WindsorContainer
        {
            public MyContainer(IConfigurationInterpreter interpreter)
            {
                // Registers the type converter:

                IConversionManager manager = (IConversionManager)
                    Kernel.GetSubSystem(Castle.MicroKernel.SubSystemConstants.ConversionManagerKey);

                manager.Add(new ServiceEntryConverter());

                Kernel.AddFacility("", new Castle.Facilities.FactorySupport.FactorySupportFacility());
                interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore);

                // Install the components                
                Installer.SetUp(this, Kernel.ConfigurationStore);
                this.Install(new MudWindsorInstaller());
                
            }
        }

        private class MudWindsorInstaller : IWindsorInstaller
        {

            public void Install(IWindsorContainer container, IConfigurationStore store)
            {

                IConversionManager manager = (IConversionManager)
                    container.Kernel.GetSubSystem(Castle.MicroKernel.SubSystemConstants.ConversionManagerKey);

                manager.Add(new ServiceEntryConverter());

                container.Kernel.AddFacility("", new Castle.Facilities.FactorySupport.FactorySupportFacility());
                container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));
                container.Kernel.Resolver.AddSubResolver(new ListResolver(container.Kernel));                
                AssemblyList.Instance.ForEach(
                    (a) =>
                    {
                        RegisterFromAssembly(container, a);
                    });

                /*
                // install default components
                AssemblyList.Instance.ForEach(
                    (a) =>
                        {
                            container.Register(
                                AllTypes.FromAssembly(a)
                                .Where((t) => (at
                 */
            }

            private static void RegisterFromAssembly(IWindsorContainer container, System.Reflection.Assembly a)
            {
                container.Register(
                AllTypes.Of<IInitializer>()
                    .FromAssembly(a)
                    .WithService.FromInterface()
                    .Configure(component
                    => component.LifeStyle.Transient.Named(component.Implementation.Name))
                );

                container.Register(
                    AllTypes.Of<ITelnetClient>()
                    .FromAssembly(a)
                    .Configure(component
                    => component.LifeStyle.Transient.Named(component.Implementation.Name))
                );
            }
        }

    }
}
