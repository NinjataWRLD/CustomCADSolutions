﻿// <auto-generated />
using System;
using CustomCADSolutions.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CustomCADSolutions.Infrastructure.Migrations
{
    [DbContext(typeof(CADContext))]
    [Migration("20231229124202_Initial_Migration")]
    partial class Initial_Migration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CustomCADSolutions.Infrastructure.Data.Models.CAD", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("CADs");
                });

            modelBuilder.Entity("CustomCADSolutions.Infrastructure.Data.Models.Order", b =>
                {
                    b.Property<int>("BuyerId")
                        .HasColumnType("int");

                    b.Property<int>("CADId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.HasKey("BuyerId", "CADId");

                    b.HasIndex("CADId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("CustomCADSolutions.Infrastructure.Data.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CustomCADSolutions.Infrastructure.Data.Models.CAD", b =>
                {
                    b.HasOne("CustomCADSolutions.Infrastructure.Data.Models.User", null)
                        .WithMany("CADs")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CustomCADSolutions.Infrastructure.Data.Models.Order", b =>
                {
                    b.HasOne("CustomCADSolutions.Infrastructure.Data.Models.User", "Buyer")
                        .WithMany("Orders")
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CustomCADSolutions.Infrastructure.Data.Models.CAD", "CAD")
                        .WithMany("Orders")
                        .HasForeignKey("CADId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Buyer");

                    b.Navigation("CAD");
                });

            modelBuilder.Entity("CustomCADSolutions.Infrastructure.Data.Models.CAD", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("CustomCADSolutions.Infrastructure.Data.Models.User", b =>
                {
                    b.Navigation("CADs");

                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
