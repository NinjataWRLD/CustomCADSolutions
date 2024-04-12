﻿namespace CustomCADSolutions.App.Mappings.CadDTOs
{
    public class CadExportDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CategoryName { get; set; } = null!;  
        public int CategoryId { get; set; } 
        public string CreatorId { get; set; } = null!;
        public string CreatorName { get; set; }  = null!;
        public string CreationDate { get; set; } = null!;
        public byte[] Bytes { get; set; } = null!;
        public bool IsValidated { get; set; }
        public decimal Price { get; set; }
        public int[] Coords { get; set; } = new int[3];
        public char? SpinAxis { get; set; }
        public byte[] RGB { get; set; } = new byte[3];
    }
}
