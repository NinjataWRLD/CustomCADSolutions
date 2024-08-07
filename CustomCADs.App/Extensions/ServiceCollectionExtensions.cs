﻿using CustomCADs.App.Extensions;
using CustomCADs.App.Hubs;
using CustomCADs.App.Resources.Shared;
using CustomCADs.Core.Contracts;
using CustomCADs.Core.Services;
using CustomCADs.Domain;
using CustomCADs.Domain.Entities.Identity;
using CustomCADs.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = config.GetConnectionString("RealConnection");
            services.AddDbContext<CadContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IRepository, Repository>();
            return services;
        }

        public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<CadContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IMvcBuilder AddInbuiltServices(this IServiceCollection services)
        {
            AutoValidateAntiforgeryTokenAttribute antiForgeryToken = new();
            var extensionFormat = LanguageViewLocationExpanderFormat.Suffix;

            return services
                .AddControllersWithViews(opt => opt.Filters.Add(antiForgeryToken))
                .AddViewLocalizer(extensionFormat, typeof(DisplayResources));
        }

        private static IMvcBuilder AddViewLocalizer(this IMvcBuilder builder, LanguageViewLocationExpanderFormat format, Type resource)
        {
            builder.AddViewLocalization(format);
            builder.AddDataAnnotationsLocalization(options =>
                options.DataAnnotationLocalizerProvider = (type, factory)
                    => factory.Create(resource));

            return builder;
        }

        public static IServiceCollection AddLocalizer(this IServiceCollection services, CultureInfo[] cultures)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");

                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });

            return services;
        }

        public static IServiceCollection AddRoles(this IServiceCollection services, IEnumerable<string> roles)
        {
            services.AddAuthorization(options =>
            {
                foreach (string role in roles)
                {
                    options.AddPolicy(role, policy => policy.RequireRole(role));
                }
            });

            return services;
        }

        public static IServiceCollection AddAbstractions(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICadService, CadService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<CadsHubHelper>();

            return services;
        }

        public static IServiceCollection AddStripe(this IServiceCollection services, IConfiguration config)
        {
            IConfigurationSection stripeSection = config.GetSection("Stripe");
            services.Configure<StripeInfo>(stripeSection);
            return services;
        }

        public static IServiceCollection AddSignalRAndHubs(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddTransient<CadsHub>();
            return services;
        }
    }
}
