﻿using AutoMapper;
using CustomCADSolutions.App.Extensions;
using CustomCADSolutions.App.Mappings;
using CustomCADSolutions.App.Mappings.DTOs;
using CustomCADSolutions.App.Models.Orders;
using CustomCADSolutions.Core.Contracts;
using CustomCADSolutions.Core.Models;
using CustomCADSolutions.Infrastructure.Data.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace CustomCADSolutions.App.APIControllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
            mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrderModelProfile>()).CreateMapper();
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult<OrderExportDTO[]>> GetAsync()
        {
            try
            {
                IEnumerable<OrderModel> orders = (await orderService.GetAllAsync())
                    .OrderBy(o => (int)o.Status).ThenBy(o => o.OrderDate);

                return mapper.Map<OrderExportDTO[]>(orders);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{buyerId}/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult<OrderExportDTO>> GetAsync(int id, string buyerId)
        {
            try
            {
                OrderModel order = await orderService.GetByIdAsync(id, buyerId);
                return mapper.Map<OrderExportDTO>(order);
            }
            catch (KeyNotFoundException)
            {
                return BadRequest();
            }
        }

        [HttpPost("Create")]
        [Consumes("application/json")]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<OrderExportDTO>> PostAsync(OrderImportDTO dto)
        {
            OrderModel model = mapper.Map<OrderModel>(dto);
            model.OrderDate = DateTime.Now;

            try
            {
                (string, int) ids = await orderService.CreateAsync(model);

                return CreatedAtAction(nameof(GetAsync),
                    new { buyerId = ids.Item1, id = ids.Item2 },
                    mapper.Map<OrderExportDTO>(model));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("Edit")]
        [Consumes("application/json")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status403Forbidden)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PutAsync(OrderImportDTO dto)
        {
            try
            {
                OrderModel order = await orderService.GetByIdAsync(dto.CadId, dto.BuyerId);
                if (order.Status != OrderStatus.Pending)
                {
                    return Forbid();
                }

                order.Cad.Name = dto.Cad.Name;
                order.Description = dto.Description;
                order.Cad.CategoryId = dto.Cad.CategoryId;
                await orderService.EditAsync(order);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{buyerId}/{id}")]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult<OrderModel>> DeleteAsync(int id, string buyerId)
        {
            if (await orderService.ExistsByIdAsync(id, buyerId))
            {
                await orderService.DeleteAsync(id, buyerId);
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("Delete2")]
        [IgnoreAntiforgeryToken]
        public IActionResult SecondDelete() => NoContent();
    }
}
