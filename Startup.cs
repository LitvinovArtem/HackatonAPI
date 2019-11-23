using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using WebDekAPI.Auth;
using Microsoft.Extensions.FileProviders;
using System.IO;
using WebDekAPI.Data;
using System.Security.Claims;
using System.Security.Principal;
using WebDekAPI.Models;
using WebDekAPI.Hubs;
using System.Diagnostics;
//using WebDekAPI.Hubs;
//using WebDekAPI.Hubs;

namespace WebDekAPI
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddSignalR();

            services.AddCors();
            services.Configure<SettingsApp>(Configuration.GetSection("Settings"));

			services.AddSingleton<ConnectionMapping<String>>();

            //services.AddHostedService<TimedHostedService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    // укзывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // строка, представляющая издателя
                    ValidIssuer = TokenAuthOption.Issuer,

                    // будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    
                    // установка потребителя токена
                    ValidAudience = TokenAuthOption.Audience,
                    
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,
                    
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,

                    // установка ключа безопасности
                    IssuerSigningKey = TokenAuthOption.GetSymmetricSecurityKey(),

                    // ClockSkew = TokenAuthOption.ExpiresSpan
                };
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = (context) =>
					{
						if (context.Request.Path.ToString().StartsWith("/api/notifications") && !context.Request.Headers.Where(w=>w.Key == "Authorization").Any())
							context.Request.Headers.Add("Authorization", context.Request.Query["token"]);
						
						return Task.CompletedTask;
					},
				};
			});


            services.AddDbContext<DataBaseContext>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // services.AddSingleton<IAuthorizationHandler, ExperienceHandler>();
            // ClaimsPrincipal User = new ClaimsPrincipal;
            //Func<IServiceProvider, IPrincipal> getPrincipal = (sp) => sp.GetService<IHttpContextAccessor>().HttpContext.User;



            //services.AddScoped(typeof(Func<IPrincipal>), getPrincipal);

            //services.AddScoped <IPrincipal, ClaimsPrincipal>();

            services.AddScoped <IRatingRepository, VedKafManager>();

            services.AddMemoryCache();

            services.AddMvc().AddXmlSerializerFormatters();


		}


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {


            #region static files
            app.UseStaticFiles();
			#endregion
			bool test = true;
            if (test || env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			
			//app.UseCors(
			//    options => options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader()
			//);

			// app.UseStatusCodePages();
			//app.UseStatusCodePages(context => context.HttpContext.Response.SendAsync("Handler, status code: " + context.HttpContext.Response.StatusCode, "text/plain"));
			//

			app.UseStatusCodePages(
                options =>
                {
                    options.Run(
                        async context =>
                        {
                            context.Response.ContentType = "application/json";
                            int status = context.Response.StatusCode;
                            var msg = (System.Net.HttpStatusCode) status;
                            var view = new RespondView(null,RequestState.Failed,  msg.ToString());
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(view)).ConfigureAwait(false);
                        });
                }
                );

            app.UseExceptionHandler(
                options => {
                    options.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "application/json";
                            var ex = context.Features.Get<IExceptionHandlerFeature>();
                            if (ex != null)
                            {
                                //ex.Error.StackTrace;
                                if (!Debugger.IsAttached)
                                {
                                    using (var db = new DataBaseContext())
                                    {
                                        int userID = authService.GetUserID(context.User);
                                        db.LogErrors.Add(new LogErrors() { computer = "webapi", date = DateTime.Now, description = context.Request.Path + context.Request.QueryString, program = "webapi", user = userID.ToString() });
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                var view = new RespondView(null, RequestState.Failed,"msg: " + ex.Error.Message + " | src: " + ex.Error.Source + " | stack" + ex.Error.StackTrace);
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(view)).ConfigureAwait(false);
                                
                            }
                        });
                }
                );


            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
			.AllowCredentials());
			


			//app.UseCors(builder =>
			//builder.WithOrigins("https://vedkaf.donstu.ru")
			//.AllowAnyMethod()
			//.AllowAnyHeader());


			//app.UseStaticFiles(new StaticFileOptions
			//{
			//    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),
			//    RequestPath = "/node_modules"
			//});

			//#region Handle Exception
			//app.UseExceptionHandler(appBuilder =>
			//{
			//    appBuilder.Use(async (context, next) =>
			//    {
			//        var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

			//        //when authorization has failed, should retrun a json message to client
			//        if (error != null && error.Error is SecurityTokenExpiredException)
			//        {
			//            context.Response.StatusCode = 401;
			//            context.Response.ContentType = "application/json";

			//            await context.Response.WriteAsync(JsonConvert.SerializeObject(new RequestResult
			//            {
			//                State = RequestState.NotAuth,
			//                Msg = "token expired"
			//            }));
			//        }
			//        //when orther error, retrun a error message json to client
			//        else if (error != null && error.Error != null)
			//        {
			//            context.Response.StatusCode = 500;
			//            context.Response.ContentType = "application/json";
			//            await context.Response.WriteAsync(JsonConvert.SerializeObject(new RequestResult
			//            {
			//                State = RequestState.Failed,
			//                Msg = error.Error.Message
			//            }));
			//        }
			//        //when no error, do next.
			//        else await next();
			//    });
			//});
			//#endregion


			app.UseAuthentication();
			app.UseSignalR(routes =>
			{
				routes.MapHub<NotificationHub>("/api/notifications");
			});
			app.UseMvc();
			//app.UseFastReport();

			
		}

        private object HandleExceptionAsync(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
