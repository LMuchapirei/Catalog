using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Catalog.Api.Repositories;
using Catalog.Api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Catalog.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            // MongoCredential credential=MongoCredential.CreateCredential(
            //     "mongodb",
            //     mongoDbSettings.User,
            //     mongoDbSettings.Password
            // );
            // var settings=new MongoClientSettings {
            //     Credential=credential,
            //     Server=new MongoServerAddress(mongoDbSettings.Host,mongoDbSettings.Port)
            // };
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                // var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                 return new MongoClient(mongoDbSettings.ConnectionString);
                // return new MongoClient(settings);
            });
            services.AddSingleton<IItemsRepository, MongoDbItemsRepository>(); // One instance through the lifetime of the application
            // To fix the issue where GetItemAsync is not being accepted add the following code to the 
            // service add Controller like so
            // the default behavior is to remove the async at runtime thats why when l removed it it worked
            services.AddControllers(
                options=>
                    options.SuppressAsyncSuffixInActionNames=false);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });

            services.AddHealthChecks()
                .AddMongoDb(
                    mongoDbSettings.ConnectionString,
                    name:"mongodbb",
                    timeout:TimeSpan.FromSeconds(3),
                    tags:new[]{"ready"});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
            }

            if (env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                endpoints.MapHealthChecks("/health/ready",
                    new HealthCheckOptions
                    {
                        Predicate = (check)=>check.Tags.Contains("ready"),
                        ResponseWriter = async (context, report) =>
                        {
                            var result = JsonSerializer.Serialize(
                                new
                                {
                                    status=report.Status.ToString(),
                                    checks=report.Entries.Select(entry =>
                                   new  {
                                        name=entry.Key,
                                        status=entry.Value.Status.ToString(),
                                        exception=entry.Value.Exception !=null?entry.Value.Exception.Message:"None",
                                        duration=entry.Value.Duration.ToString()
                                    })
                                });
                            context.Response.ContentType = MediaTypeNames.Application.Json;
                            await context.Response.WriteAsync(result);
                        }
                    });
                endpoints.MapHealthChecks("/health/live",
                    new HealthCheckOptions
                    {
                        Predicate = (_)=>false
                    });
                
            });
        }
    }
}
