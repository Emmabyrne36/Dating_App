﻿using System.Net;
using System.Text;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup (IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            // services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))); // edit this to change the db provider
            services.AddDbContext<DataContext> (x =>
                x.UseSqlServer (Configuration.GetConnectionString ("DefaultConnection")));
            // .ConfigureWarnings (warnings => warnings.Ignore (CoreEventId.IncludeIgnoredWarning)));

            services.AddControllers ()
                .AddNewtonsoftJson (opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            // services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2)
            //     .AddJsonOptions (opts =>
            //     {
            //         opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //     });
            services.AddCors ();
            services.Configure<CloudinarySettings> (Configuration.GetSection ("CloudinarySettings"));
            services.AddAutoMapper (typeof (DatingRepository).Assembly);
            services.AddTransient<Seed> ();
            // created once per request withing the current scope
            services.AddScoped<IAuthRepository, AuthRepository> ();
            services.AddScoped<IDatingRepository, DatingRepository> ();
            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer (options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes (Configuration.GetSection ("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                    };
                });
            services.AddScoped<LogUserActivity> ();
        }

        // This configuration used in development mode
        public void ConfigureDevelopmentServices (IServiceCollection services)
        {
            services.AddDbContext<DataContext> (x => x.UseSqlite (Configuration.GetConnectionString ("DefaultConnection"))); // edit this to change the db provider

            services.AddControllers ()
                .AddNewtonsoftJson (opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            // services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1)
            //     .AddJsonOptions (opts =>
            //     {
            //         opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //     });

            // services.BuildServiceProvider().GetService<DataContext>().Database.Migrate(); // applies migrations to db
            services.AddCors ();
            services.Configure<CloudinarySettings> (Configuration.GetSection ("CloudinarySettings"));
            services.AddAutoMapper (typeof (DatingRepository).Assembly);
            services.AddTransient<Seed> ();
            // created once per request withing the current scope
            services.AddScoped<IAuthRepository, AuthRepository> ();
            services.AddScoped<IDatingRepository, DatingRepository> ();
            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer (options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes (Configuration.GetSection ("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                    };
                });
            services.AddScoped<LogUserActivity> ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment ())
            {
                app.UseDeveloperExceptionPage ();
            }
            else
            {
                app.UseExceptionHandler (builder =>
                {
                    builder.Run (async context =>
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature> ();
                        if (error != null)
                        {
                            context.Response.AddApplicationError (error.Error.Message);
                            await context.Response.WriteAsync (error.Error.Message);
                        }
                    });
                });
                // app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseRouting ();
            app.UseAuthentication ();
            app.UseAuthorization ();
            app.UseCors (x => x.AllowAnyOrigin ().AllowAnyMethod ().AllowAnyHeader ());
            app.UseDefaultFiles (); // looks for a default file eg: index.html
            app.UseStaticFiles (); // to use the Angular SPA

            app.UseEndpoints (endpoints =>
            {
                endpoints.MapControllers ();
                endpoints.MapFallbackToController ("Index", "Fallback");
            });
        }
    }
}