﻿using AutoMapper;
using CustomCADSolutions.Core.Contracts;
using CustomCADSolutions.Core.Models;
using CustomCADSolutions.Infrastructure.Data.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using CustomCADSolutions.API.Mappings;
using CustomCADSolutions.API.Models.Cads;
using CustomCADSolutions.API.Models.Orders;
using Microsoft.AspNetCore.Authorization;

namespace CustomCADSolutions.API.Controllers
{
    [Authorize(Roles = "Client,Contributor")]
    [ApiController]
    [Route("API/[controller]")]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        private readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<OrderApiProfile>();
            cfg.AddProfile<CadApiProfile>();
        }).CreateMapper();

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

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult<OrderExportDTO>> GetSingleAsync(int id)
        {
            try
            {
                OrderModel order = await orderService.GetByIdAsync(id);
                return mapper.Map<OrderExportDTO>(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult<OrderExportDTO>> PostAsync(OrderImportDTO dto)
        {
            OrderModel model = mapper.Map<OrderModel>(dto);
            model.OrderDate = DateTime.Now;
            
            try
            {
                int id = await orderService.CreateAsync(model);

                model = await orderService.GetByIdAsync(id);
                OrderExportDTO export = mapper.Map<OrderExportDTO>(model);

                return CreatedAtAction(null, new { id }, export);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status403Forbidden)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PutAsync(int id, OrderImportDTO dto)
        {
            try
            {
                OrderModel order = await orderService.GetByIdAsync(id);

                if (dto.BuyerId != order.BuyerId)
                {
                    return Forbid();
                }

                order.Name = dto.Name;
                order.Description = dto.Description;
                order.Status = Enum.Parse<OrderStatus>(dto.Status);
                order.ShouldShow = dto.ShouldShow;
                order.Name = dto.Name;
                order.CategoryId = dto.CategoryId;
                await orderService.EditAsync(id, order);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPatch("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PatchAsync(int id, [FromBody] JsonPatchDocument<OrderModel> patchOrder)
        {
            try
            {
                OrderModel model = await orderService.GetByIdAsync(id);
                patchOrder.ApplyTo(model);

                IList<string> errors = orderService.ValidateEntity(model);
                if (errors.Any())
                {
                    return BadRequest(string.Join("; ", errors));
                }

                await orderService.EditAsync(id, model);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
            catch
            {
                return StatusCode(Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult<OrderModel>> DeleteAsync(int id)
        {
            try
            {
                if (await orderService.ExistsByIdAsync(id))
                {
                    await orderService.DeleteAsync(id);
                    return NoContent();
                }
                else return NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("Finish/{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult> FinishAsync(int id, CadImportDTO dto)
        {
            if (!await orderService.ExistsByIdAsync(id))
            {
                return BadRequest();
            }
            OrderModel order = (await orderService.GetByIdAsync(id))!;

            order.Cad = new()
            {
                Name = dto.Name,
                IsValidated = dto.IsValidated,
                Price = dto.Price,
                CreationDate = DateTime.Now,
                CreatorId = dto.CreatorId,
                CategoryId = dto.CategoryId,
            };
            await orderService.FinishOrderAsync(id, order);

            return NoContent();
        }
    }
}
