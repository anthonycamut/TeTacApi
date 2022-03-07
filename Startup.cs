using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TeTacApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSwag.Generation.Processors.Security;
using NSwag;

namespace TeTacApi
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
      // Add Cors
      services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
      {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()               
               .AllowAnyHeader();
      }));      
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidIssuer = Configuration["Jwt:Issuer"],
          ValidAudience = Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))          
        };
      });

      services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      services.AddDbContext<TeTacApiContext>(opt =>
               opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
      services.AddDbContext<TeTacApiContextSalon>();
      services.AddControllers();
      // Register the Swagger services
      services.AddSwaggerDocument(config =>
      {
        // Adds the "Authorization" parameter in the request header, to authorize access to the APIs
        //config.OperationProcessors.Add(new AddRequiredHeaderParameter());

        config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
        config.AddSecurity("JWT Token", Enumerable.Empty<string>(),
            new OpenApiSecurityScheme()
            {
              Type = OpenApiSecuritySchemeType.ApiKey,
              Name = "Authorization",
              In = OpenApiSecurityApiKeyLocation.Header,
              Description = "Copy the token retrieved from login method into the value field: Bearer {token}"
            }
        );

        config.PostProcess = document =>
        {
          document.Info.Version = "v1";          
          document.Info.Title = "TeTac API";
          document.Info.Description = "Tech-Event TAC API - JWT token authentication - First of all call Login method with the credential you have received, then use the token generated to authorize API call";          
          document.Info.TermsOfService = "None";          
          document.Info.Contact = new NSwag.OpenApiContact
          {
            Name = "Tech-Event",
            Email = "contact@tech-event.net",
            Url = "http://www.tech-event.net"
          };          
        };
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
      
      //app.UseMiddleware<MyMiddleware>();
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

   
      

      app.UseRouting();

      // Enable Cors
      app.UseCors("MyPolicy");
      app.UseAuthentication();

      app.UseAuthorization();
      app.UseHttpsRedirection();



      // Register the Swagger generator and the Swagger UI middlewares
      app.UseOpenApi();
      app.UseSwaggerUi3(a =>
      {
        a.TagsSorter = "alpha";
      }
        );

      // Logger
      loggerFactory.AddFile("Logs/mylog-{Date}.txt");

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers().RequireCors("MyPolicy");
      });
    }
  }



}
