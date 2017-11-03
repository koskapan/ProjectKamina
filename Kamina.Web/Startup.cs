using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Kamina.Web.App_Start;
using Owin;
using Unity;
using Unity.AspNet.WebApi;

namespace Kamina.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);
            
            var unityContainer = new UnityContainer();

            UnityConfig.ConfigureContainer(unityContainer);

            config.DependencyResolver = new UnityDependencyResolver(unityContainer);

            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;

            jsonFormatter.UseDataContractJsonSerializer = true;


            builder.UseWebApi(config);

        }
    }
}