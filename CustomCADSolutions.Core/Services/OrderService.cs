﻿using CustomCADSolutions.Core.Contracts;
using CustomCADSolutions.Core.Models;
using CustomCADSolutions.Infrastructure.Data.Common;
using CustomCADSolutions.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomCADSolutions.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository repository;
        private readonly IConverter converter;

        public OrderService(IRepository repository, IConverter converter)
        {
            this.repository = repository;
            this.converter = converter;
        }

        public async Task CreateAsync(OrderModel model)
        {
            if (model.Cad == null || model.Buyer == null)
            {
                throw new NullReferenceException();
            }
            Order order = converter.ModelToOrder(model);

            await repository.AddAsync<Order>(order);
            await repository.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(params OrderModel[] models)
        {
            List<Order> orders = new();
            foreach (OrderModel model in models)
            {
                if (model.Cad == null || model.Buyer == null)
                {
                    throw new NullReferenceException();
                }
                Order order = converter.ModelToOrder(model);
                orders.Add(order);
            }
            await repository.AddRangeAsync<Order>(orders.ToArray());
            await repository.SaveChangesAsync();
        }

        public async Task EditAsync(OrderModel model)
        {
            Order order = repository
                .All<Order>()
                .FirstOrDefault(order => order.CadId == model.CadId
                                      && order.BuyerId == model.BuyerId)
                ?? throw new KeyNotFoundException();


            order.Description = model.Description;
            order.Status = model.Status;
            order.Cad.Name = model.Cad.Name;
            order.Cad.Category = model.Cad.Category;
            order.Cad.CreationDate = model.Cad.CreationDate;

            await repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int cadId, string buyerId)
        {
            Order order = repository
                .All<Order>()
                .FirstOrDefault(order => order.CadId == cadId
                                      && order.BuyerId == buyerId)
                ?? throw new KeyNotFoundException();
            
            repository.Delete(order);
            Cad? cad = await repository.GetByIdAsync<Cad>(cadId);
            if (cad != null)
            {
                repository.Delete<Cad>(cad);
            }

            await repository.SaveChangesAsync();
        }

        public async Task<OrderModel> GetByIdAsync(int cadId, string buyerId)
        {
            Order order = await repository
                .GetByIdAsync<Order>(cadId, buyerId)
                ?? throw new KeyNotFoundException();

            OrderModel model = converter.OrderToModel(order);
            return model;
        }
        
        public async Task<IEnumerable<OrderModel>> GetAllAsync()
        {
            return await repository.All<Order>()
                .Select(o => converter.OrderToModel(o, true))
                .ToArrayAsync();
        }
    }
}
