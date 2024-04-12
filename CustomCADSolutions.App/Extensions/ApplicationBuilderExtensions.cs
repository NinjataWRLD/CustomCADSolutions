﻿using CustomCADSolutions.App.Extensions;
using CustomCADSolutions.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLocalizion(this IApplicationBuilder app, string defaultCulture, params CultureInfo[] supportedCultures)
        {
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            return app;
        }

        public static IApplicationBuilder UseCorsOrigins(this IApplicationBuilder app, params string[] origins)
        {
            app.UseCors(opt => opt.WithOrigins(origins));
            return app;
        }

        public static IApplicationBuilder UseProductionMiddlewares(this IApplicationBuilder app)
        {
            string exceptionPath = "/Home/StatusCodeHandler";

            app.UseHsts();
            app.UseExceptionHandler(exceptionPath);
            app.UseStatusCodePagesWithReExecute(exceptionPath, "?statusCode={0}");
            return app;
        }

        public static IApplicationBuilder UseDevelopmentMiddlewares(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }

        public static async Task<IServiceProvider> UseRolesAsync(this IServiceProvider service, string[] roles)
        {
            using IServiceScope scope = service.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRole(role));
                }
            }

            return service;
        }

        public static async Task<IServiceProvider> UseAppUsers(this IServiceProvider service, IConfiguration config, Dictionary<string, string> users)
        {
            using IServiceScope scope = service.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            int i = 1;
            foreach (KeyValuePair<string, string> user in users)
            {
                string role = user.Key;
                string username = user.Value;
                string email = $"user{i++}@gmail.com";
                string password = config[$"Passwords:{role}"];
                await userManager.AddUserAsync(username, email, password, role);
            }
            
            return service;
        }

        public static IApplicationBuilder MapRoutes(this WebApplication app)
        {
            app.MapAreaControllerRoute(
                name: "AdminArea",
                areaName: "Admin",
                pattern: "Admin/{controller}/{action=Index}/{id?}");

            app.MapAreaControllerRoute(
                name: "DesignerArea",
                areaName: "Designer",
                pattern: "Designer/{controller}/{action=Index}/{id?}");

            app.MapAreaControllerRoute(
                name: "ContributorArea",
                areaName: "Contributor",
                pattern: "Contributor/{controller=Home}/{action=Index}/{id?}");

            app.MapAreaControllerRoute(
                name: "ClientArea",
                areaName: "Client",
                pattern: "Client/{controller=Home}/{action=Index}/{id?}");

            app.MapAreaControllerRoute(
                name: "IdentityArea",
                areaName: "Identity",
                pattern: "Identity/{controller=Account}/{action=Register}");

            app.MapDefaultControllerRoute();

            return app;
        }
    }
}
