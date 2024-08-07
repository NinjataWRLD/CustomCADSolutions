﻿using CustomCADs.Core.Models.Orders;
using CustomCADs.Domain.Entities.Enums;
using CustomCADs.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using static CustomCADs.Domain.DataConstants;

namespace CustomCADs.Core.Models.Cads
{
    public class CadModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(CadConstants.NameMaxLength, MinimumLength = CadConstants.NameMinLength, ErrorMessage = LengthErrorMessage)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(CadConstants.DescriptionMaxLength, MinimumLength = CadConstants.DescriptionMinLength, ErrorMessage = LengthErrorMessage)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = RequiredErrorMessage)]
        public CadStatus Status { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        [Range(CadConstants.PriceMin, CadConstants.PriceMax)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public double[] Coords { get; set; } = new double[3];

        [Required(ErrorMessage = RequiredErrorMessage)]
        public double[] PanCoords { get; set; } = new double[3];

        public string CadPath { get; set; } = string.Empty;
        public string CadExtension => '.' + CadPath.Split('.')[^1].ToLower();

        public string ImagePath { get; set; } = string.Empty;
        public string ImageExtension => '.' + ImagePath.Split('.')[^1].ToLower();

        [Required(ErrorMessage = RequiredErrorMessage)]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public string CreatorId { get; set; } = null!;

        public CategoryModel Category { get; set; } = null!;

        public AppUser Creator { get; set; } = null!;

        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
    }
}
