﻿using AutoMapper;
using CustomCADs.App.Extensions;
using CustomCADs.App.Hubs;
using CustomCADs.App.Mappings;
using CustomCADs.App.Models.Cads.Input;
using CustomCADs.App.Models.Cads.View;
using CustomCADs.Core.Contracts;
using CustomCADs.Core.Models;
using CustomCADs.Core.Models.Cads;
using CustomCADs.Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using static CustomCADs.App.Extensions.UtilityExtensions;
using static CustomCADs.Domain.DataConstants.RoleConstants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CustomCADs.App.Controllers
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
        public async Task<IActionResult> Index([FromQuery] CadQueryInputModel queryModel)
        {
            CadQuery query = new()
            {
                Creator = User.Identity!.Name,
            };
            SearchModel search = new()
            {
                Category = queryModel.Category,
                Name = queryModel.SearchName,
                Sorting = queryModel.Sorting.ToString(),
            };
            PaginationModel pagination = new()
            {
                Page = queryModel.CurrentPage,
                Limit = queryModel.CadsPerPage,
            };

            CadResult result = await cadService.GetAllAsync(query, search, pagination);

            queryModel.Categories = await categoryService.GetAllNamesAsync();
            queryModel.TotalCount = result.Count;
            queryModel.Cads = mapper.Map<CadViewModel[]>(result.Cads);

            ViewBag.Sortings = typeof(Sorting).GetEnumNames();
            ViewBag.Category = queryModel.Category;
            return View(queryModel);
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
            model.CreationDate = DateTime.Now;
            model.Status = User.IsInRole(Designer)
                ? CadStatus.Validated 
                : CadStatus.Unchecked;

            try
            {
                if (input.CadFolder != null)
                {
                    string[] cadFormats = [".gltf", ".glb"];
                    IFormFile cad = input.CadFolder!
                        .Single(f => cadFormats.Contains(f.GetFileExtension()));

                    int cadId = await cadService.CreateAsync(model);
                    string cadPath = await env.UploadCadFolderAsync(cad, model.Name + cadId, input.CadFolder.Where(f => f != cad))
                        ?? throw new NullReferenceException("Cad is null!");
                    await cadService.SetPathsAsync(cadId, cadPath, "fix l8r");
                }
                else if (input.CadFile != null)
                {
                    int cadId = await cadService.CreateAsync(model);
                    string cadPath = await env.UploadCadAsync(input.CadFile, input.Name + cadId + input.CadFile.GetFileExtension())
                        ?? throw new NullReferenceException("Cad is null!");
                    await cadService.SetPathsAsync(cadId, cadPath, "fix l8r");
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

            if (cad.CadExtension != ".glb")
            {
                string path = Regex.Match(cad.CadPath, $"others/cads/{cad.Name}{cad.Id}").Value;
                env.DeleteFolder(path);
            }
            else env.DeleteFile(cad.CadPath);

            await cadService.DeleteAsync(id);
            await statisticsService.SendStatistics(User.GetId());

            return RedirectToAction(nameof(Index));
        }
    }
}
