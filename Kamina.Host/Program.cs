using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kamina.Web;
using Microsoft.Owin.Hosting;


namespace Kamina.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = "http://localhost:1313";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Service started");

                Console.WriteLine("Press any key to stop");

                Console.ReadKey();

            }
        }
    }
}
