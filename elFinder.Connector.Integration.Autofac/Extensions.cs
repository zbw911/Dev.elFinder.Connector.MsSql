using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Dev.Framework.FileServer;
using Dev.Framework.FileServer.Config;
using Dev.Framework.FileServer.ImageFile;
using elFinder.Connector.Command;
using elFinder.Connector.Config;
using elFinder.Connector.MsSql;
using elFinder.Connector.Service;
using Dev.Framework.FileServer.LocalUploaderFileImpl;
using Dev.Framework.FileServer.DocFile;

namespace elFinder.Connector.Integration.Autofac
{
    public static class Extensions
    {
        public static void RegisterElFinderConnectorDefault(this ContainerBuilder builder)
        {
            builder.RegisterElFinderConnectorCore();

            builder.RegisterElFinderConnectorServices<DefaultVolumeManager, FileServerImageEditorService, Base64CryptoService>(AppConnectorConfig.Instance);

            builder.RegFileServer();
        }

        public static void RegisterElFinderConnectorCore(this ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ICommand).Assembly)
                .Where(t => t.IsAssignableTo<ICommand>() && !t.IsAbstract)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            //builder.RegisterAssemblyTypes(typeof(IVolume).Assembly)
            //    .Where(t => t.IsAssignableTo<IVolume>() && !t.IsAbstract)
            //    .AsImplementedInterfaces()
            //    .InstancePerDependency();

            builder.RegisterType<MsSqlVolume>().As<IVolume>().InstancePerDependency();




            builder.RegisterType<DefaultCheckAuth>().As<ICheckAuth>().InstancePerDependency();
        }


        public static void RegFileServer(this ContainerBuilder builder)
        {
            var x = new ReadConfig("LocalUploadFile.config");

            builder.RegisterType<LocalFileKey>().As<IKey>().InstancePerDependency();
            builder.RegisterType<LocalUploadFile>().As<IUploadFile>().InstancePerDependency();
            builder.RegisterType<DocFileUploader>().As<IDocFile>().InstancePerDependency();
            builder.RegisterType<ImageUploader>().As<IImageFile>().InstancePerDependency();
            //this.Kernel.Bind<IKey>().To<LocalFileKey>().InRequestScope();

            //this.Kernel.Bind<IUploadFile>().To<LocalUploadFile>().InRequestScope();


            ////文档类型
            //this.Kernel.Bind<IDocFile>().To<DocFileUploader>().InRequestScope();
            ////图片类型
            //this.Kernel.Bind<IImageFile>().To<ImageUploader>().InRequestScope();
        }

        public static void RegisterElFinderConnectorServices<TConfig, TVolumeManager, TImageEditorService, TCryptoService>(this ContainerBuilder builder)
            where TConfig : IConnectorConfig
            where TVolumeManager : IVolumeManager
            where TImageEditorService : IImageEditorService
            where TCryptoService : ICryptoService
        {
            builder.RegisterType<TConfig>()
                .As<IConnectorConfig>().SingleInstance();

            builder.RegisterType<TVolumeManager>()
                .As<IVolumeManager>().SingleInstance();

            builder.RegisterType<TImageEditorService>()
                .As<IImageEditorService>().SingleInstance();

            builder.RegisterType<TCryptoService>()
                .As<ICryptoService>().SingleInstance();
        }

        public static void RegisterElFinderConnectorServices<TVolumeManager, TImageEditorService, TCryptoService>(this ContainerBuilder builder, IConnectorConfig configInstance)
            where TVolumeManager : IVolumeManager
            where TImageEditorService : IImageEditorService
            where TCryptoService : ICryptoService
        {
            builder.Register(c => configInstance)
                .As<IConnectorConfig>().SingleInstance();

            builder.RegisterType<TVolumeManager>()
                .As<IVolumeManager>().SingleInstance();

            builder.RegisterType<TImageEditorService>()
                .As<IImageEditorService>().SingleInstance();

            builder.RegisterType<TCryptoService>()
                .As<ICryptoService>().SingleInstance();
        }

        public static void SetAsElFinderDependencyResolver(this IContainer contaner)
        {
            DependencyResolver.SetResolver(new AutofacDependencyResolver(contaner));
        }
    }
}
