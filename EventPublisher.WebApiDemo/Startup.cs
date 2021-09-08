
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.KeyManagementService;
using Amazon.Kinesis;
using Amazon.Runtime;
using System;
using EventPublisher.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EventPublisher.WebApiDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventPublisher.WebApiDemo", Version = "v1" });
            });
            
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Test")
            {
                var hostEnv = Environment.GetEnvironmentVariable("HOST_ENV") ?? "localhost";
                var awsOptions = new AWSOptions
                {
                    Credentials = new BasicAWSCredentials("foo", "bar"),
                    Region = RegionEndpoint.APSoutheast2,
                    DefaultClientConfig =
                    {
                        ServiceURL = hostEnv == "localhost" ? "http://localhost:4566/" : "http://localstack:4566/"
                    }
                };
                services.AddAWSService<IAmazonKeyManagementService>(awsOptions);
            }
            else
            {
                services.AddAWSService<IAmazonKeyManagementService>();
            }

            // TODO: Think about cases that config options come from different sources.
            // Add EventBus extension
            services.UseEventBus(Configuration.GetSection("EventBus"));
            // Or configure this way.
            services.UseEventBus<PaymentsEventBusOptions>(option =>
            {
                option.AccessKeyId = Configuration.GetSection("EventBus:AccessKeyId").Value;
                option.SecretAccessKey = Configuration.GetSection("EventBus:SecretAccessKey").Value;
                option.ServerUrl = Configuration.GetSection("EventBus:ServerUrl").Value;
                option.StreamName = Configuration.GetSection("EventBus:StreamName").Value;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventPublisher.WebApiDemo v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}