using System;
using System.IO;
using System.Reflection;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MovieRank.Data;
using MovieRank.Libs.Mappers;
using MovieRank.Libs.Repositories;
using MovieRank.Services;

namespace MovieRank
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAWSService<IAmazonDynamoDB>();
            services.AddDefaultAWSOptions(
                new AWSOptions
                {
                    Region = RegionEndpoint.GetBySystemName("eu-central-1")
                });
            //MovieRankService : IMovieRankService
            services.AddScoped<IMovieRankService, MovieRankService>();
            services.AddScoped<IMovieRankRepository, NoDocumentModelRepository>();
            services.AddScoped<IMapper, Mapper>();
            //services.AddScoped<IDynamoDBContext, DynamoDBContext>();
            services.AddTransient<Seeder>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "'MovieRank' DynamoDB WebApi",
                    Description = "You Should create a 'MovieRank' DynamoDB with Table name MovieRank," +
                                  "Primary partition key UserId (Number), Primary sort key	MovieName (String)," +
                                  "\n\n\nDO NOT Forget to configure your account locally with 'aws configure' and give access to the user:\n\n" +
                                  "\n\n\nTABLE STRUCTURE:\n\n" +
                                  "{\"Actors\":{\"SS\":[\"Jemma Sung\",\"Steve Bose\"]},\"Description\":{\"S\":\"" +
                                  "It was a day blala\"},\"MovieName\":{\"S\":\"Fallen Names\"},\"RankedDateTime\":{\"S\":\"2020-02-29T06:37:08.908Z\"}," +
                                  "\"Ranking\":{\"N\":\"8\"},\"UserId\":{\"N\":\"1\"}}",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "You are DOOMED",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/spboyer"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seeder seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //A simple seeder look at the JsonSeeder.template
            seeder.Run(seed: false);

            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
