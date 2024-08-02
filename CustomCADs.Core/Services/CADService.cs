﻿using AutoMapper;
using CustomCADs.Core.Contracts;
using CustomCADs.Core.Mappings;
using CustomCADs.Core.Models;
using CustomCADs.Core.Models.Cads;
using CustomCADs.Domain;
using CustomCADs.Domain.Entities;
using CustomCADs.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;


namespace CustomCADs.Core.Services
{
    public class CadService(IRepository repository) : ICadService
    {
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryCoreProfile>();
                cfg.AddProfile<CadCoreProfile>();
                cfg.AddProfile<OrderCoreProfile>();
            }).CreateMapper();

        public async Task<CadResult> GetAllAsync(CadQuery query, SearchModel search, PaginationModel pagination, Expression<Func<Cad, bool>>? customFilter = null)
        {
            IQueryable<Cad> allCads = repository.All<Cad>();

            // Querying
            if (query.Creator != null)
            {
                allCads = allCads.Where(c => c.Creator!.UserName == query.Creator);
            }
            if (query.Status != null)
            {
                allCads = allCads.Where(c => c.Status == query.Status);
            }

            // Optional custom filter
            if (customFilter != null)
            {
                allCads = allCads.Where(customFilter);
            }

            // Search & Sort
            if (search.Category != null)
            {
                allCads = allCads.Where(c => c.Category.Name == search.Category);
            }
            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                allCads = allCads.Where(c => c.Name.Contains(search.Name));
            }
            if (!string.IsNullOrWhiteSpace(search.Owner))
            {
                allCads = allCads.Where(c => c.Creator.UserName!.Contains(search.Owner));
            }

            allCads = search.Sorting.ToLower() switch
            {
                "newest" => allCads.OrderByDescending(c => c.CreationDate),
                "oldest" => allCads.OrderBy(c => c.CreationDate),
                "alphabetical" => allCads.OrderBy(c => c.Name),
                "unalphabetical" => allCads.OrderByDescending(c => c.Name),
                "category" => allCads.OrderBy(m => m.Category.Name),
                _ => allCads.OrderBy(c => c.Id),
            };

            Cad[] cads = await allCads
                .Skip((pagination.Page - 1) * pagination.Limit)
                .Take(pagination.Limit)
                .ToArrayAsync();

            CadModel[] models = mapper.Map<CadModel[]>(cads);
            return new()
            {
                Count = (await allCads.ToArrayAsync()).Length,
                Cads = models,
            }; 
        }

        public async Task<CadModel> GetByIdAsync(int id)
        {
            Cad cad = await repository.GetByIdAsync<Cad>(id)
                ?? throw new KeyNotFoundException($"Model with id: {id} doesn't exist");

            CadModel model = mapper.Map<CadModel>(cad);
            return model;
        }

        public async Task<bool> ExistsByIdAsync(int id)
            => await repository.GetByIdAsync<Cad>(id) != null;

        public int Count(Func<CadModel, bool> predicate)
        {
            return repository.Count<Cad>(cad => predicate(mapper.Map<CadModel>(cad)));
        }

        public async Task SetPathsAsync(int id, string cadPath, string imagePath)
        {
            Cad cad = await repository.GetByIdAsync<Cad>(id)
                ?? throw new KeyNotFoundException("No such Cad exists.");

            cad.CadPath = cadPath;
            cad.ImagePath = imagePath;
            await repository.SaveChangesAsync();
        }

        public IList<string> ValidateEntity(CadModel model)
        {
            List<ValidationResult> validationResults = [];
            if (!Validator.TryValidateObject(model, new(model), validationResults, true))
            {
                return validationResults.Select(result => result.ErrorMessage ?? string.Empty).ToList();
            }

            return [];
        }

        public async Task<int> CreateAsync(CadModel model)
        {
            Cad cad = mapper.Map<Cad>(model);
            EntityEntry<Cad> entry = await repository.AddAsync<Cad>(cad);
            await repository.SaveChangesAsync();

            return entry.Entity.Id;
        }

        public async Task EditAsync(int id, CadModel model)
        {
            Cad cad = await repository.GetByIdAsync<Cad>(id)
                ?? throw new KeyNotFoundException("Model doesn't exist!");

            cad.Name = model.Name;
            cad.Description = model.Description;
            cad.Price = model.Price;
            cad.CategoryId = model.CategoryId;

            cad.X = model.Coords[0];
            cad.Y = model.Coords[1];
            cad.Z = model.Coords[2];
            cad.PanX = model.PanCoords[0];
            cad.PanY = model.PanCoords[1];
            cad.PanZ = model.PanCoords[2];

            await repository.SaveChangesAsync();
        }
        
        public async Task EditStatusAsync(int id, CadStatus status)
        {
            Cad cad = await repository.GetByIdAsync<Cad>(id)
                ?? throw new KeyNotFoundException("Model doesn't exist!");

            cad.Status = status;
            await repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            IQueryable<Order> orders = repository.All<Order>()
                .Where(o => o.CadId == id);

            foreach (Order order in orders)
            {
                order.Status = OrderStatus.Pending;
                order.DesignerId = null;
                order.Designer = null;
                order.CadId = null;
                order.Cad = null;
            }

            Cad cad = await repository.GetByIdAsync<Cad>(id)
                ?? throw new KeyNotFoundException();

            repository.Delete<Cad>(cad);
            await repository.SaveChangesAsync();
        }
    }
}
