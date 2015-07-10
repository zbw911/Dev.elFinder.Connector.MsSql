using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using Autofac;
using elFinder.MVCTest2.Core;
using LookJs;

namespace LookJs
{
    public class Global : HttpApplication
    {
        private static IContainer _container;

      

        public static void RegisterRoutes(RouteCollection routes)
        {
          

        }

        protected void Application_Start()
        {
           

            // register IoC
            var builder = new ContainerBuilder();
            builder.RegisterElFinderConnectorDefault();
            _container = builder.Build();
            // need also to set container in elFinder module
            _container.SetAsElFinderDependencyResolver();

            
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
