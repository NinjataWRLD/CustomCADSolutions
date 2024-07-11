﻿using CustomCADs.Infrastructure.Data.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace CustomCADs.API.Helpers
{
    public static class Utilities
    {
        public static async Task SignIn(this AppUser user, SignInManager<AppUser> signInManager, AuthenticationProperties? authprop)
            => await signInManager.Context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                await signInManager.CreateUserPrincipalAsync(user),
                authprop);
    }
}
