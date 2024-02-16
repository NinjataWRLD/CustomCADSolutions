﻿namespace CustomCADSolutions.Infrastructure.Constants
{
    public static class DataConstants
    {
        public const string RequiredErrorMessage = "{0} is required!";
        public const string LengthErrorMessage = "{0} length must be between {2} and {1} characters";
        public const string RangeErrorMessage = "{0} must be between {2} and {1}";

        public class CadConstants
        {

            public const string NameDisplay = "Name";
            public const int NameMaxLength = 18;
            public const int NameMinLength = 2;

            public const string CategoryDisplay = "Category";

            public const string FileDisplay = "3D Model";

            public const int XMin = -1000;
            public const int XMax = 1000;

            public const int YMin = -1000;
            public const int YMax = 1000;
            
            public const int ZMin = -1000;
            public const int ZMax = 1000;
            
            public const string SpinAxisErrorMessage = "{0} must be either x, y or z";
            
            public const double SpinFactorMin = 0.01;
            public const double SpinFactorMax = 1;
        }

        public class OrderConstants
        {
            public const string NameDisplay = "Name of 3D Model";

            public const string CategoryDisplay = "Category of 3D Model";

            public const string DescriptionDisplay = "Full Description of 3D Model";
            public const int DescriptionMaxLength = 1500;
            public const int DescriptionMinLength = 5;
        }
    }
}
