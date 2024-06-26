﻿using CustomCADSolutions.App.Models.Cads.View;
using CustomCADSolutions.Core.Contracts;
using static CustomCADSolutions.Infrastructure.Data.DataConstants.RoleConstants;
using Microsoft.AspNetCore.Mvc;

namespace CustomCADSolutions.App.Components
{
    public class CategoryCadComponent() : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(CadViewModel cad)
        {
            ViewBag.Buttons = new string[] { "Order" };

            bool isLoggedIn = User.Identity?.IsAuthenticated ?? false;
            if (isLoggedIn)
            {
                if (User.IsInRole(Client))
                {
                    ViewBag.Area = "Client";
                }
                else if (User.IsInRole(Contributor))
                {
                    ViewBag.Area = "Contributor";
                }
                else if (User.IsInRole(Designer))
                {
                    ViewBag.Area = "Designer";
                }
            }
            ViewBag.IsLoggedIn = isLoggedIn;

            return await Task.FromResult<IViewComponentResult>(View(cad));
        }
    }
}
