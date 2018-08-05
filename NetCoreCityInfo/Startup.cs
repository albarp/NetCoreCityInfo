using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreCityInfo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.Run(async (context) =>
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("IsDevelopment: {0} <br/>", env.IsDevelopment());
                sb.AppendFormat("IsProduction: {0} <br/>", env.IsProduction());
                sb.AppendFormat("IsStaging: {0} ", env.IsStaging());


                string output = sb.ToString();

                await context.Response.WriteAsync(output);
            });
        }
    }
}
