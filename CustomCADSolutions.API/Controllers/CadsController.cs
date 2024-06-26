﻿using AutoMapper;
using CustomCADSolutions.Core.Contracts;
using CustomCADSolutions.Core.Models;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using CustomCADSolutions.API.Mappings;
using CustomCADSolutions.API.Models.Cads;
using Microsoft.AspNetCore.Authorization;

namespace CustomCADSolutions.API.Controllers
{
    [Authorize(Roles = "Contributor")]
    [ApiController]
    [Route("API/[controller]")]
    public class CadsController(ICadService cadService) : ControllerBase
    {
        private readonly IMapper mapper = new MapperConfiguration(cfg 
                => cfg.AddProfile<CadApiProfile>())
            .CreateMapper();

        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(Status200OK)]
        public async Task<ActionResult<CadQueryDTO>> GetAsync([FromQuery] CadQueryModel query)
        {
            if (query.CadsPerPage % 3 != 0)
            {
                query.CadsPerPage = 3 * (query.CadsPerPage / 3);
            }
            return mapper.Map<CadQueryDTO>(await cadService.GetAllAsync(query));
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult<CadExportDTO>> GetAsync(int id)
        {
            try
            {
                CadModel model = await cadService.GetByIdAsync(id);
                return mapper.Map<CadExportDTO>(model);
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
        public async Task<ActionResult> PostAsync(CadImportDTO import)
        {
            CadModel model = mapper.Map<CadModel>(import);
            model.CreationDate = DateTime.Now;

            try
            {
                int id = await cadService.CreateAsync(model);

                model = await cadService.GetByIdAsync(id);
                CadExportDTO export = mapper.Map<CadExportDTO>(model);

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
        public async Task<ActionResult> PutAsync(int id, CadImportDTO dto)
        {
            try
            {
                CadModel cad = await cadService.GetByIdAsync(id);
                
                cad.Name = dto.Name;
                cad.CategoryId = dto.CategoryId;
                cad.Coords = dto.Coords;
                cad.PanCoords = dto.PanCoords;
                cad.Price = dto.Price;
                await cadService.EditAsync(id, cad);

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
        public async Task<ActionResult> PatchAsync(int id, [FromBody] List<Operation<CadModel>> operations)
        {
            JsonPatchDocument<CadModel> patchCad = new(operations, new DefaultContractResolver());
            try
            {
                CadModel model = await cadService.GetByIdAsync(id);
                patchCad.ApplyTo(model);

                IList<string> errors = cadService.ValidateEntity(model);
                if (errors.Any())
                {
                    return BadRequest(string.Join("; ", errors));
                }

                await cadService.EditAsync(id, model);
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
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                if (await cadService.ExistsByIdAsync(id))
                {
                    await cadService.DeleteAsync(id);
                    return NoContent();
                }
                else return NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
