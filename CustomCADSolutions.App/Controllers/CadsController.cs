﻿using static CustomCADSolutions.App.Extensions.UtilityExtensions;
using CustomCADSolutions.App.Extensions;
using CustomCADSolutions.App.Mappings;
using CustomCADSolutions.App.Models.Cads.Input;
using CustomCADSolutions.App.Models.Cads.View;
using CustomCADSolutions.Core.Models;
using CustomCADSolutions.Core.Contracts;
using CustomCADSolutions.Infrastructure.Data.Models.Enums;
using static CustomCADSolutions.Infrastructure.Data.DataConstants.RoleConstants;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomCADSolutions.App.Hubs;
using System.Text.RegularExpressions;

namespace CustomCADSolutions.App.Controllers
{
    [Authorize(Roles = $"{Contributor},{Designer}")]
    public class CadsController(
        ICadService cadService,
        ICategoryService categoryService,
        CadsHubHelper statisticsService,
        IWebHostEnvironment env,
        ILogger<CadsController> logger) : Controller
    {
        private readonly ILogger logger = logger;
        private readonly IMapper mapper = new MapperConfiguration(cfg
                    => cfg.AddProfile<CadAppProfile>())
                .CreateMapper();

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateCoords(int id, int x, int y, int z, int panx, int pany, int panz)
        {
            CadModel model = await cadService.GetByIdAsync(id);
            model.Coords = [x, y, z];
            model.PanCoords = [panx, pany, panz];

            await cadService.EditAsync(id, model);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] CadQueryInputModel query)
        {
            // Ensuring cads per page are divisible by the count of columns
            if (query.CadsPerPage % query.Cols != 0)
            {
                query.CadsPerPage = query.Cols * (query.CadsPerPage / query.Cols);
            }

            CadQueryResult result = await cadService.GetAllAsync(new()
            {
                Category = query.Category,
                SearchName = query.SearchName,
                Sorting = query.Sorting,
                CurrentPage = query.CurrentPage,
                CadsPerPage = query.CadsPerPage,
                Creator = User.Identity!.Name,
                Validated = true,
                Unvalidated = true,
            });

            query.Categories = await categoryService.GetAllNamesAsync();
            query.TotalCount = result.TotalCount;
            query.Cads = mapper.Map<CadViewModel[]>(result.Cads);

            ViewBag.Sortings = typeof(CadSorting).GetEnumNames();
            ViewBag.Category = query.Category;
            return View(query);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            CadModel model = await cadService.GetByIdAsync(id);
            return View(mapper.Map<CadViewModel>(model));
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(new CadAddModel()
            {
                Categories = await categoryService.GetAllAsync()
            });
        }

        [HttpPost]
        [RequestSizeLimit(150_000_000)]
        public async Task<IActionResult> Add(CadAddModel input)
        {
            if (!ModelState.IsValid)
            {
                input.Categories = await categoryService.GetAllAsync();
                return View(input);
            }

            CadModel model = mapper.Map<CadModel>(input);
            model.CreatorId = User.GetId();
            model.IsValidated = User.IsInRole(Designer);
            model.CreationDate = DateTime.Now;

            try
            {
                if (input.CadFolder != null)
                {
                    string[] cadFormats = [".gltf", ".glb"];
                    IFormFile cad = input.CadFolder!
                        .Single(f => cadFormats.Contains(f.GetFileExtension()));

                    model.IsFolder = true;
                    model.Extension = cad.GetFileExtension();
                    int cadId = await cadService.CreateAsync(model);
                    string cadPath = await env.UploadCadFolderAsync(cad, model.Name + cadId, input.CadFolder.Where(f => f != cad))
                        ?? throw new NullReferenceException("Cad is null!");
                    await cadService.SetPathAsync(cadId, cadPath);
                }
                else if (input.CadFile != null)
                {
                    model.Extension = input.CadFile.GetFileExtension();
                    int cadId = await cadService.CreateAsync(model);
                    string cadPath = await env.UploadCadAsync(input.CadFile, input.Name + cadId + input.CadFile.GetFileExtension())
                        ?? throw new NullReferenceException("Cad is null!");
                    await cadService.SetPathAsync(cadId, cadPath);
                }
                else throw new ArgumentNullException("Upload either File or Folder");
            }
            catch (NullReferenceException)
            {
                input.Categories = await categoryService.GetAllAsync();
                return View(input);
            }
            catch (ArgumentNullException)
            {
                input.Categories = await categoryService.GetAllAsync();
                return View(input);
            }

            await statisticsService.SendStatistics(User.GetId());
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CadModel model = await cadService.GetByIdAsync(id);
            if (model.CreatorId != User.GetId())
            {
                return Forbid("You don't have access to this model");
            }

            CadEditModel input = mapper.Map<CadEditModel>(model);
            input.Categories = await categoryService.GetAllAsync();

            return View(input);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CadEditModel input)
        {
            CadModel model = await cadService.GetByIdAsync(id);
            model.Name = input.Name;
            model.CategoryId = input.CategoryId;
            model.Price = input.Price;

            await cadService.EditAsync(id, model);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            CadModel cad = await cadService.GetByIdAsync(id);

            if (cad.IsFolder)
            {
                string path = Regex.Match(cad.Path, $"others/cads/{cad.Name}{cad.Id}").Value;
                env.DeleteFolder(path);
            }
            else env.DeleteFile(cad.Path);

            await cadService.DeleteAsync(id);
            await statisticsService.SendStatistics(User.GetId());

            return RedirectToAction(nameof(Index));
        }
    }
}
