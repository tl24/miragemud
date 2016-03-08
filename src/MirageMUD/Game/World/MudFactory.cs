using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;
using Mirage.Core;
using Mirage.Core.Collections;
using Mirage.Game.IO.Net;
using Mirage.Core.IO.Net;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Facilities.FactorySupport;
using System.Configuration;
using Mirage.Game.Server;
using Mirage.Game.Communication;
using Mirage.Game.World.Skills;
using Castle.Facilities.Startable;
using Castle.Facilities.Logging;
using Mirage.Game.Command.ArgumentConversion;
using Mirage.Core.Command;
using Mirage.Core.Command.ArgumentConversion;
using Mirage.Game.Command;
using Mirage.Core.Server;

namespace Mirage.Game.World
{
    /// <summary>
    /// Abstraction class for creating and referencing types
    /// </summary>
    public class MudFactory
    {
        private static WindsorContainer _instance;

        static MudFactory()
        {
            _instance = new MyContainer();            
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
            public MyContainer()
            {
                // Registers the type converter:

                //IConversionManager manager = (IConversionManager)
                //    Kernel.GetSubSystem(Castle.MicroKernel.SubSystemConstants.ConversionManagerKey);

                Kernel.AddFacility(new FactorySupportFacility());
                Kernel.AddFacility<StartableFacility>();
                Kernel.AddFacility(new LoggingFacility(LoggerImplementation.Log4net));
                //interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore, Kernel);

                // Install the components                
                //Installer.SetUp(this, Kernel.ConfigurationStore);
                this.Install(new MudWindsorInstaller());
                
            }
        }

        private class MudWindsorInstaller : IWindsorInstaller
        {

            public void Install(IWindsorContainer container, IConfigurationStore store)
            {

                IConversionManager manager = (IConversionManager)
                    container.Kernel.GetSubSystem(Castle.MicroKernel.SubSystemConstants.ConversionManagerKey);

                container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));
                container.Kernel.Resolver.AddSubResolver(new ListResolver(container.Kernel));                
                AssemblyList.Instance.ForEach(
                    (a) =>
                    {
                        RegisterFromAssembly(container, a);
                    });

                // I/O
                var textClientRegistration = Component.For<IConnectionListener>()
                                    .ImplementedBy<ClientListener<TextConnection>>()
                                    //.Named("TextClientListener")
                                    .DependsOnAppSetting(typeof(int), "textclient.port", "port")
                                    .OptionallyDependsOnAppSetting(typeof(string), "textclient.host", "host");
                var guiClientRegistration = Component.For<IConnectionListener>()
                                    .ImplementedBy<ClientListener<AdvancedConnection>>()
                                    //.Named("GuiClientListener")
                                    .DependsOnAppSetting(typeof(int), "guiclient.port", "port")
                                    .OptionallyDependsOnAppSetting(typeof(string), "guiclient.host", "host");
                container.Register(textClientRegistration, guiClientRegistration);
                container.Register(Component.For<ConnectionManager>()
                                    //.Named("ClientManager")
                                    .OptionallyDependsOnAppSetting(typeof(int), "clientmanager.maxthreads", "maxthreads"));

                container.Register(Component.For<IClientFactory>().ImplementedBy<ClientFactory>());
                container.Register(Component.For<ICombatModule>().ImplementedBy<DefaultCombatModule>());
                container.Register(Component.For<ServiceProcessor>());
                container.Register(Component.For<MirageServer>());
                container.Register(Component.For<IRaceRepository>().ImplementedBy<RaceRepository>());
                container.Register(Component.For<MudWorld>()); // check channel dependency
			    container.Register(Component.For<IPlayerRepository>().ImplementedBy<PlayerRepository<Player>>());
			    container.Register(Component.For<IAreaRepository>().ImplementedBy<AreaRepository<Area>>());
			    container.Register(Component.For<IChannelRepository>().ImplementedBy<ChannelRepository>());
			    container.Register(Component.For<IMobileRepository>().ImplementedBy<MobileRepository>());
			    container.Register(Component.For<ISkillRepository>().ImplementedBy<SkillRepository>());
			    container.Register(Component.For<IViewManager>().ImplementedBy<ViewManager>().LifestyleTransient());
                container.Register(Component.For<IReflectedCommandFactory>().ImplementedBy<ReflectedCommandGroupFactory>());
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
                    .LifestyleTransient()                    
                );

                container.Register(
                    Classes.FromAssembly(a)
                    .BasedOn<IConnection>()
                    .Configure(component
                    => component.LifeStyle.Transient.Named(component.Implementation.Name))
                );

                container.Register(
                    Classes.FromAssembly(a)
                    .BasedOn<IClient<ClientPlayerState>>()
                    .Configure(component
                    => component.LifeStyle.Transient.Named(component.Implementation.Name))
                );

                container.Register(
                    Classes.FromAssembly(a)
                    .BasedOn<CustomAttributeConverter>()
                    .WithServiceBase());

            }
        }

    }
}
