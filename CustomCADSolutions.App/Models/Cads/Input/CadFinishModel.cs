﻿using CustomCADSolutions.App.Resources.Shared;
using CustomCADSolutions.Infrastructure.Data.Models;
using static CustomCADSolutions.Infrastructure.Data.DataConstants;
using System.ComponentModel.DataAnnotations;

namespace CustomCADSolutions.App.Models.Cads.Input
{
    public class CadFinishModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(SharedResources),
            ErrorMessageResourceName = nameof(SharedResources.Required))]
        [StringLength(CadConstants.NameMaxLength,
            MinimumLength = CadConstants.NameMinLength,
            ErrorMessageResourceType = typeof(SharedResources),
            ErrorMessageResourceName = nameof(SharedResources.Length))]
        [Display(Name = "Name", ResourceType = typeof(SharedResources))]
        public string Name { get; set; } = null!;

        [Display(Name = "Description", ResourceType = typeof(SharedResources))]
        public string Description { get; set; } = null!;

        [Required(ErrorMessageResourceType = typeof(SharedResources),
            ErrorMessageResourceName = nameof(SharedResources.Required))]
        [Range(CadConstants.PriceMin, CadConstants.PriceMax,
            ErrorMessageResourceType = typeof(SharedResources),
            ErrorMessageResourceName = nameof(SharedResources.Range))]
        [Display(Name = "Price", ResourceType = typeof(SharedResources))]
        public decimal Price { get; set; }

        [Required(ErrorMessageResourceType = typeof(SharedResources),
            ErrorMessageResourceName = nameof(SharedResources.Required))]
        [Display(Name = "File", ResourceType = typeof(SharedResources))]
        public IFormFile CadFile { get; set; } = null!;

        public int OrderId { get; set; }

        [Required(ErrorMessageResourceType = typeof(SharedResources),
            ErrorMessageResourceName = nameof(SharedResources.Required))]
        [Display(Name = "Category", ResourceType = typeof(SharedResources))]
        public int CategoryId { get; set; }

        public IEnumerable<Category>? Categories { get; set; }
    }
}