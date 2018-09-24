using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreCityInfo.Entities;
using NetCoreCityInfo.Services;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;

namespace NetCoreCityInfo
{
    public class Startup
    {
        // Non mi convince metterlo static, ma in questo modo è raggingibile in tutta l'applicazione
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(o => {
                    // se vogliamo che i nomi delle proprietà json siano uguali alle classi c# 
                    // (altrimenti newtonsoft applicherà il pascalCase
                    //var castedResover = o.SerializerSettings.ContractResolver as DefaultContractResolver;
                    //castedResover.NamingStrategy = null;
                })
                .AddMvcOptions(o => {
                    // Se vogliamo il risultato in Json
                    //o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    o.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
                })
            ;

            services.AddTransient<IMailService, LocalMailService>();


            var connectionString = @"Server=(localdb)\mssqllocaldb;Database=CityInfoDB;Trusted_Connection=True;";

            // default: scoped
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));

            // If stdoutLogEnabled="true" and stdoutLogFile=".\logs\stdout" in web.config, then
            // it gets logged
            Console.WriteLine("ConfigureServices called");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Env variables
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-2.1

            // Logging configuration
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1
            var logConf = Configuration["Logging:IncludeScopes"];

            // Std logger
            //var logger = loggerFactory.CreateLogger(this.GetType());
            //logger.LogWarning("Warning");

            

            //var generalLogger = ApplicationLogging.CreateLogger("General");

            //generalLogger.LogCritical("from static");

            // Questi due, invece, ce li mette già il default bilder in program.cs
            //loggerFactory.AddConsole();
            //loggerFactory.AddDebug();

            // Questo è per aggiungere NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");

            // Custom static logger
            ApplicationLogging.ConfigureLogger(loggerFactory);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }


            app.UseStatusCodePages();

            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    StringBuilder sb = new StringBuilder();

            //    sb.AppendFormat("IsDevelopment: {0} <br/>", env.IsDevelopment());
            //    sb.AppendFormat("IsProduction: {0} <br/>", env.IsProduction());
            //    sb.AppendFormat("IsStaging: {0} ", env.IsStaging());


            //    string output = sb.ToString();

            //    await context.Response.WriteAsync(output);
            //});

            // If stdoutLogEnabled="true" and stdoutLogFile=".\logs\stdout" in web.config, then
            // it gets logged
            Console.WriteLine("Configure called");
        }
    }
}
