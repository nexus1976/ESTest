[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ESTest.Api.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(ESTest.Api.App_Start.NinjectWebCommon), "Stop")]

namespace ESTest.Api.App_Start
{
    using ESTest.DAL;
    using ESTest.Domain;
    using Hubs;
    using Microsoft.AspNet.SignalR;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System;
    using System.Web;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                GlobalHost.DependencyResolver = new NinjectSignalRDependencyResolver(kernel);
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IQueryContext>().To<Context>();
            kernel.Bind<ICommandContext>().To<Context>();
            kernel.Bind<IUserSummaryRepository>().To<UserSummaryRepository>()
                .WithConstructorArgument("queryContext", context => context.Kernel.Get<IQueryContext>());
            kernel.Bind<IMessageRepository>().To<MessageRepository>()
                .WithConstructorArgument("queryContext", context => context.Kernel.Get<IQueryContext>())
                .WithConstructorArgument("commandContext", context => context.Kernel.Get<ICommandContext>());
            kernel.Bind<IConversationRepository>().To<ConversationRepository>()
                .WithConstructorArgument("queryContext", context => context.Kernel.Get<IQueryContext>())
                .WithConstructorArgument("commandContext", context => context.Kernel.Get<ICommandContext>());
            kernel.Bind<IConversationBoundedContext>().To<ConversationBoundedContext>()
                .WithConstructorArgument("conversationRepository", context => context.Kernel.Get<IConversationRepository>())
                .WithConstructorArgument("userSummaryRepository", context => context.Kernel.Get<IUserSummaryRepository>())
                .WithConstructorArgument("messageRepository", context => context.Kernel.Get<IMessageRepository>());
        }        
    }
}
