﻿using CustomCADSolutions.Infrastructure.Data.Models.Enums;
using static CustomCADSolutions.Infrastructure.Constants.DataConstants;
using System.ComponentModel.DataAnnotations;

namespace CustomCADSolutions.App.Models
{
    public class CadInputModel
    {
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(Cad.NameMaxLength, 
            MinimumLength = Cad.NameMinLength, 
            ErrorMessage = LengthErrorMessage)]
        [Display(Name = Cad.NameDisplay)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [Display(Name = Cad.CategoryDisplay)]
        public Category Category { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        [Display(Name = Cad.FileDisplay)]
        public IFormFile File { get; set; } = null!;
    }
}
