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

            // Per gestire le stringhe di connessione, come altri dati sensibili, si parte dall'ambiente (deciso 
            // dalla variabile d'ambiente):
            // 
            // 1. Development: in questo caso le stringhe di connessione, che puntano a db locali non sensibili, vanno nell'appsettings.json
            //
            // 2. Stagin/Production: le stringhe di connessione, che adesso sono sensibili, vanno nelle variabili d'ambiente.
            //      la variabile d'ambiente si deve chiamare come la chiave utilizzata per recuperare la configurazione
            //          "connectionStrings:cityInfoDBConnectionString" in questo caso. Il valore, invece, deve essere la stringa di
            //          connessione (Server=(localdb)\\mssqllocaldb;Database=CityInfoDB;Trusted_Connection=True;)
            //
            // In ogni caso il valore si recupera sempre dalla configurazione:

            var connectionString = Configuration["connectionStrings:cityInfoDBConnectionString"];

            //var connectionString = @"Server=(localdb)\mssqllocaldb;Database=CityInfoDB;Trusted_Connection=True;";

            // default: scoped
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<ICityInfoRepository, CityInfoRepository>();

            // If stdoutLogEnabled="true" and stdoutLogFile=".\logs\stdout" in web.config, then
            // it gets logged
            Console.WriteLine("ConfigureServices called");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            CityInfoContext cityInfoContext)
        {

            AutoMapper.Mapper.Initialize(Cfg =>
            {
                Cfg.CreateMap<Entities.City, Models.CityWithoutPointOfInterestDto>();
                Cfg.CreateMap<Entities.City, Models.CityDto>();
                Cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
                Cfg.CreateMap<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>();
                Cfg.CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>();
                Cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>();
            });

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

            cityInfoContext.EnsureSeedDataForContext();

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
