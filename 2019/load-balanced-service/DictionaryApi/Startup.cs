using System;
using Jaeger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;

namespace DictionaryApi
{
    public class Startup
    {
        private readonly IHostingEnvironment env;

        public Startup(IConfiguration configuration,IHostingEnvironment env)
        {
            this.env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<ITracer>(cli =>
            {
                Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", "dictionary-api");

                //if (env.IsDevelopment())
                {
                    Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", "jaeger-agent");
                    Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", "6831");
                    Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "const");
                }

                var loggerFactory = new LoggerFactory();

                var config = Jaeger.Configuration.FromEnv(loggerFactory);
                var tracer = config.GetTracer();

               /* var sampler = new Configuration.SamplerConfiguration(loggerFactory).WithType("const").WithParam(1);
                var reporter = new Configuration.ReporterConfiguration(loggerFactory).WithLogSpans(true);

                var config1 = new Jaeger.Configuration("dictionary-api", loggerFactory)
                    .WithReporter(reporter)
                    .WithSampler(sampler);
*/
                if (!GlobalTracer.IsRegistered())
                {
                    // Allows code that can't use DI to also access the tracer.
                    GlobalTracer.Register(tracer);
                }

                return tracer;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
