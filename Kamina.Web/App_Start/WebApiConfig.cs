﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Kamina.Web.App_Start;
using Unity;
using Unity.AspNet.WebApi;

namespace Kamina.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            var unityContainer = new UnityContainer();

            UnityConfig.ConfigureContainer(unityContainer);

            config.DependencyResolver = new UnityDependencyResolver(unityContainer);

            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;

            jsonFormatter.UseDataContractJsonSerializer = true;

        }
    }
}