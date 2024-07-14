﻿using CustomCADs.Core.Contracts;
using CustomCADs.Infrastructure.Data.Models;
using CustomCADs.Infrastructure.Data;
using CustomCADs.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;
using CustomCADs.Core.Services;
using AutoMapper;
using CustomCADs.Core.Mappings;
using CustomCADs.Core.Models;

namespace CustomCADs.Tests.ServicesTests.CategoryTests
{
    public class BaseCategoriesTests
    {
        private IRepository repository;
        private IMapper mapper = new MapperConfiguration(cfg => 
                cfg.AddProfile<CategoryCoreProfile>())
            .CreateMapper();

        protected ICategoryService service;
        protected readonly CategoryModel[] categories = new CategoryModel[5]
        {
            new() { Id = 1, Name = "Category1" },
            new() { Id = 2, Name = "Category2" },
            new() { Id = 3, Name = "Category3" },
            new() { Id = 4, Name = "Category4" },
            new() { Id = 5, Name = "Category5" }
        };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var options = new DbContextOptionsBuilder<CadContext>()
                .UseInMemoryDatabase("CadCategoriesContext").Options;

            this.repository = new Repository(new(options));
            this.service = new CategoryService(repository);
        }

        [SetUp]
        public async Task Setup()
        {
            Category[] categories = mapper.Map<Category[]>(this.categories);
            await repository.AddRangeAsync(categories);
            await repository.SaveChangesAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            Category[] allCategories = await repository.All<Category>().ToArrayAsync();
            repository.DeleteRange(allCategories);
            await repository.SaveChangesAsync();
        }
    }
}