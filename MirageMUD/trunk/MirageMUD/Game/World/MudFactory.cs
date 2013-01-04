using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;
using Mirage.Core.Collections;
using Mirage.Game.IO.Net;
using Mirage.IO.Net;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Facilities.FactorySupport;

namespace Mirage.Game.World
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
            _instance.Register(Component.For(classType).Named(classType.FullName));
        }

        private class MyContainer : WindsorContainer
        {
            public MyContainer(IConfigurationInterpreter interpreter)
            {
                // Registers the type converter:

                IConversionManager manager = (IConversionManager)
                    Kernel.GetSubSystem(Castle.MicroKernel.SubSystemConstants.ConversionManagerKey);

                manager.Add(new ServiceEntryConverter());

                Kernel.AddFacility(new FactorySupportFacility());
                interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore, Kernel);

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

                //container.Kernel.AddFacility(new FactorySupportFacility());
                container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));
                container.Kernel.Resolver.AddSubResolver(new ListResolver(container.Kernel));                
                AssemblyList.Instance.ForEach(
                    (a) =>
                    {
                        RegisterFromAssembly(container, a);
                    });

                container.Register(Component.For<IConnectionAdapterFactory>().ImplementedBy<ConnectionAdapterFactory>());
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
                    Classes.FromAssembly(a)
                    .BasedOn<IInitializer>()
                    .WithService.FromInterface()
                    .Configure(component
                    => component.LifeStyle.Transient.Named(component.Implementation.Name))
                );

                container.Register(
                    Classes.FromAssembly(a)
                    .BasedOn<IConnection>()
                    .Configure(component
                    => component.LifeStyle.Transient.Named(component.Implementation.Name))
                );

                container.Register(
                    Classes.FromAssembly(a)
                    .BasedOn<IConnectionAdapter>()
                    .Configure(component
                    => component.LifeStyle.Transient.Named(component.Implementation.Name))
                );

            }
        }

    }
}
