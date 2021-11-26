using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BackendAPI
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
            services.AddCors(o => o.AddPolicy("ReactPolicy", builder =>
            {
                builder.AllowAnyOrigin()            //change to: .WithOrigins("http://example.com", "http://www.contoso.com");
                       .AllowAnyMethod()
                       .AllowAnyHeader();
                       //.AllowCredentials();
            }));


            //services.AddMvc().AddNewtonsoftJson();

            //Maybe best to move code with GroupsController - Get()
            //Groups: See: https://stackoverflow.com/questions/19467673/entity-framework-self-referencing-loop-detected
            services.AddControllers().AddNewtonsoftJson(opt => {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            //portservices.AddControllers();
            
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = "XXXXXXXXXXX";    // Configuration["Authentication:Twitter:"]; // ConsumerAPIKey"];
                twitterOptions.ConsumerSecret = "XXXXXXXXXX";    // Configuration["Authentication:Twitter:"]; // ConsumerSecret"];
                twitterOptions.RetrieveUserDetails = true;
            })
            .AddCookie();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //LoggerFactory loggerFactory = new LoggerFactory();
            //loggerFactory.AddDebug();

            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}

            //app.UseDatabaseErrorPage();

            app.UseCors("ReactPolicy");

            app.UseRouting();

            app.UseAuthentication();        // .UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
