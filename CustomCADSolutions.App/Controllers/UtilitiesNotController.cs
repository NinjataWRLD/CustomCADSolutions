﻿using CustomCADSolutions.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace CustomCADSolutions.App.Controllers
{
    [NonController]
    public static class UtilitiesNotController
    {
        public static string GetId(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static string GetCadPath(this IWebHostEnvironment hostingEnvironment, string name, int id, string extension = ".stl")
            => Path.Combine(hostingEnvironment.WebRootPath, "others", "cads", $"{name}{id}{extension}");

        public static async Task<int> GetUserModelsCountAsync(this ICadService cadService, string userId) 
            => (await cadService.GetAllAsync()).Count(model => model.CreatorId == userId);

        public static IEnumerable<string> GetErrors(this ModelStateDictionary model) => model.Values.Select(v => v.Errors).SelectMany(ec => ec.Select(e => e.ErrorMessage));

        public static async Task UploadCadAsync(this IWebHostEnvironment hostingEnvironment, IFormFile cad, int id, string name, string extension = ".stl")
        {
            string filePath = hostingEnvironment.GetCadPath(name, id, extension);
            using FileStream fileStream = new(filePath, FileMode.Create);
            await cad.CopyToAsync(fileStream);
        }

        public static void EditCad(this IWebHostEnvironment hostingEnvironment, int id, string oldName, string newName)
        {
            string source = hostingEnvironment.GetCadPath(oldName, id);
            string destination = hostingEnvironment.GetCadPath(newName, id);
            File.Move(source, destination);
        }

        public static void DeleteCad(this IWebHostEnvironment hostingEnvironment, string name, int id)
        {
            string filePath = hostingEnvironment.GetCadPath(name, id);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
