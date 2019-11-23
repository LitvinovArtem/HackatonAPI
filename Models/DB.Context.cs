using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebDekAPI.Models.ViewModel.UserInfo;
using System.Diagnostics;
using System.Xml.Linq;
using WebDekAPI.Models.WebDekAPI;
using WebDekAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace WebDekAPI
{
    
    public class DataBaseContext : DbContext
    {
        public string cn;
        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });
        public DataBaseContext() 
        {

        }
        public DataBaseContext(string server, string dataBase, int timeOut = 10)
        {
            this.cn = string.Format("Data Source = {0}; Initial Catalog = {1}; Persist Security Info = True; User ID = {2}; Password = {3};Connection Timeout={4}", server, dataBase, user, pass, timeOut);
        }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) 
        {


        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(cn))
            {
                optionsBuilder.UseSqlServer(cn);
                return;
            }
            //Console.OutputEncoding = Encoding.UTF8;
            //optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            var config = new ConfigurationBuilder()
			  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			  .AddJsonFile("appsettings.json")
			  .Build();
			if (!Debugger.IsAttached)
			config = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.production.json")
              .Build();
                optionsBuilder.UseSqlServer("СТРОКА");
                
            
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
			//builder.Entity<nirFiles>().HasOne(p => p.workID).WithOne(i => i.)
			base.OnModelCreating(builder);
        }
    }
}
