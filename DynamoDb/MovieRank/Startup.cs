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
using MovieRank.Contracts;
using MovieRank.Data;
using MovieRank.Infrastructure.Mappers;
using MovieRank.Infrastructure.Repositories;
using MovieRank.Services;
using ISeederService = MovieRank.Data.ISeederService;

namespace MovieRank
{
    public class Startup
    {
        private const string Uri = "http://localhost:8000";
        private readonly IHostingEnvironment _environment;
        public Startup(IHostingEnvironment environment)
        {
            _environment = environment;
        }
       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //Use local DynamoDB for development
            if (_environment.IsDevelopment())
            {
                services.AddScoped<IAmazonDynamoDB>(cc => new AmazonDynamoDBClient(new AmazonDynamoDBConfig()
                {
                    ServiceURL = Uri
                }));
            }
            else
            {
                services.AddAWSService<IAmazonDynamoDB>();
                services.AddDefaultAWSOptions(
                    new AWSOptions
                    {
                        Region = RegionEndpoint.GetBySystemName("eu-central-1")
                    });
            }

            //Operation over RankMovie table
            services.AddScoped<IMovieRankService, MovieRankService>();

            //DDL Operations Creation and deletion of the table
            services.AddTransient<ISetupService, SetupService>();


            //1- Persistence Object Model Approach
            //services.AddScoped<IMovieRankRepository, PersistenceObjectModelRepository>();
            //services.AddScoped<IMapper, Mapper>();


            //2- Document Model Approach
            //services.AddScoped<IDocumentMapper, DocumentMapper>();
            //services.AddScoped<IMovieRankRepository, DocumentModelRepository>();

            //3- Low Level Model Approach
            services.AddScoped<ILowLevelModelMapper, LowLevelModelMapper>();
            services.AddScoped<IMovieRankRepository, LowLevelModelRepository>();

            //Create and delete table
            services.AddScoped<IDDLMovieRankRepository, DDLMovieRankRepository>();

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

            services.AddTransient<ISeederService, SeederService>();
            services.AddTransient<Seeder>();
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
