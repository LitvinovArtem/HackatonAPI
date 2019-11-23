using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebDekAPI
{
  
    public class Program
    {
        public static void Main(string[] args)
        {
            //if(File.Exists("api\\webappUpdate.zip"))
            //{
            //    UpdateWebapp();
            //    try
            //    {
            //        File.Delete("api\\webappUpdate.zip");
            //    }
            //    catch
            //    {
            //        File.Move("api\\webappUpdate.zip", "Trash\\webappUpdate.zip");
            //    }
            //}
            BuildWebHost(args).Run();
        }

        private static void UpdateWebapp()
        {
            ZipFile.ExtractToDirectory("api\\webappUpdate.zip", "WebAppTemp",true);
            if (File.Exists("WebApp\\Config.json"))
                File.Copy("WebApp\\Config.json", "WebAppTemp\\Config.json", true);
            if (File.Exists("WebApp\\favicon.ico"))
                File.Copy("WebApp\\favicon.ico", "WebAppTemp\\favicon.ico", true);
            if (Directory.Exists("WebApp"))
                Directory.Delete("WebApp", true);
            Directory.Move("WebAppTemp", "WebApp");
            //if(File.Exists("WebApp\\Config.json"))
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
