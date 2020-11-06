using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TestSolution.Api.Data;
using TestSolution.Api.Helpers;

namespace TestSolution.Api
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
            //Application database context
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["ConnectionString"]));
            //Swagger configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TEST SOLUTION API",
                    Description = "Provides all endpoints.",
                    TermsOfService = new Uri("http://company.com/"),
                    Contact = new OpenApiContact { Name = "COMPANY", Email = "info@company.com", Url = new Uri("http://company.com/") }
                });
                //Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                //https://www.thecodebuzz.com/jwt-authorization-token-swagger-open-api-asp-net-core-3-0/
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            //Swagger Newtonsoft Support => https://dotnetcoretutorials.com/2020/01/31/using-swagger-in-net-core-3/
            services.AddSwaggerGenNewtonsoftSupport();
            //JWT Authentication https://auth0.com/blog/securing-asp-dot-net-core-2-applications-with-jwts/
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"]))
                    };
                });
            //Response compression configuration => https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-2.1
            services.AddResponseCompression();
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Fastest;
            });
            //Default ASP.NET Core with JSON formatting => https://dotnetcoretutorials.com/2019/12/19/using-newtonsoft-json-in-net-core-3-projects/
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "dd/MM/yyyy HH:mm";
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()); //https://gist.github.com/regisdiogo/27f62ef83a804668eb0d9d0f63989e3e
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Errors configuration
            app.UseDeveloperExceptionPage();
            //Authentication configuration
            app.UseAuthentication();
            //Static files configuration: http://fiyazhasan.me/story-of-file-uploading-in-asp-net-core-part-iii-streaming-files/
            app.UseStaticFiles();
            //Swagger configuration
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api-explorer";
                // Adding the relative path for the virtual directory: https://github.com/domaindrivendev/Swashbuckle/issues/971
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "V1");
            });
            //Response compression configuration => https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-2.1
            app.UseResponseCompression();
            //Default ASP.NET Core
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}