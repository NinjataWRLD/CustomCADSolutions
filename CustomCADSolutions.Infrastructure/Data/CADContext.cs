﻿using CustomCADSolutions.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCADSolutions.Infrastructure.Data
{
    public class CADContext : DbContext
    {
        public CADContext()
        {
            
        }

        public CADContext(DbContextOptions<CADContext> options)
            : base(options)
        {
        }

        public DbSet<Cad> CADs { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=NINJATABG\\SQLEXPRESS;Database=CustomCADSolutions;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().Navigation<Cad>(o => o.Cad).AutoInclude();
            modelBuilder.Entity<Order>().Navigation<User>(o => o.Buyer).AutoInclude();
        }
    }
}
