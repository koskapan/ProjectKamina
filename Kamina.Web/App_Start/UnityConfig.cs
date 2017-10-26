using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Kamina.BL.Models;
using Kamina.BL.Services;
using Unity;
using Unity.Injection;

namespace Kamina.Web.App_Start
{
    public class UnityConfig
    {
        public static void ConfigureContainer(UnityContainer container)
        {

            container.RegisterType<IMaterialsService, MaterialsService>();

            var configSettings = ConfigurationManager.AppSettings["filesLocation"];

            var settings = new FileSaverSettings()
            {
                Location = configSettings
            };

            container.RegisterType<IFileSaver, LocalFileSaver>(new InjectionConstructor(settings));

        }

    }
}