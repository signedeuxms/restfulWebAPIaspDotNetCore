using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkyAPI.Data;
using ParkyAPI.Mappers;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ParkyAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the 
        // container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddDbContext<ParkyAPIdbContext>( options => 
                     options.UseSqlServer(Configuration.GetConnectionString(
                     "DefaultConnection")));

            services.AddScoped<INationalParkRepository, NationalParkRepository>();

            services.AddScoped<ITrailRepository, TrailRepository>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddAutoMapper(typeof(ParkyMappings));

            services.AddApiVersioning( options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(options => 
                                            options.GroupNameFormat = "'v'VVV");

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(auth =>
           {
               auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
           })
           .AddJwtBearer( bear => {
               bear.RequireHttpsMetadata = false;
               bear.SaveToken = true;
               bear.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false
               };
            });

            // https://localhost:44315/swagger/ParkyOpenAPIspec/swagger.json
            // open API to get available data
            //services.AddSwaggerGen( options => {
            //    options.SwaggerDoc("ParkyOpenAPIspecNationalPark",
            //    new Microsoft.OpenApi.Models.OpenApiInfo()
            //    {
            //        Title = "Parky API (National Park)",
            //        Version = "1",
            //        Description = "List of some national park to discover",
            //        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            //        {
            //            Email = "godgoodness@yahoo.com",
            //            Name = "God Goodness",
            //            Url = new Uri("https://www.bhurgen.com")
            //        },
            //        License = new Microsoft.OpenApi.Models.OpenApiLicense()
            //        {
            //            Name = "MIT License",
            //            Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
            //        }
            //    });

            //   options.SwaggerDoc("ParkyOpenAPIspecTrail",
            //   new Microsoft.OpenApi.Models.OpenApiInfo()
            //   {
            //       Title = "Parky API (Trail)",
            //       Version = "1",
            //       Description = "List of some trails to join to.",
            //       Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            //       {
            //           Email = "godgoodness@yahoo.com",
            //           Name = "God Goodness",
            //           Url = new Uri("https://www.bhurgen.com")
            //       },
            //       License = new Microsoft.OpenApi.Models.OpenApiLicense()
            //       {
            //           Name = "MIT License",
            //           Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
            //       }
            //   });

            //    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
            //    options.IncludeXmlComments(cmlCommentsFullPath);
            //});

            services.AddControllers();
        }


        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP 
        /// request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
                                IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI( options => { 
                foreach(var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }

                options.RoutePrefix = "";
            });
            // https://localhost:44315/swagger/index.html
            // https://localhost:44315/index.html
            //app.UseSwaggerUI( options => {
            //    options.SwaggerEndpoint("/swagger/ParkyOpenAPIspec/swagger.json",
            //                            "Parky API");
            //    /*options.SwaggerEndpoint("/swagger/ParkyOpenAPIspecNationalPark/swagger.json",
            //                            "Parky API National Park");
            //    options.SwaggerEndpoint("/swagger/ParkyOpenAPIspecTrail/swagger.json",
            //                            "Parky API Trail");*/
            //    options.RoutePrefix = "";
            //});

            app.UseRouting();

            // get one entrypoint to the api
            app.UseCors(cor => cor.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()) ;

            // authentication is always before the authorization
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
