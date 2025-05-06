using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MathTutor.Core.Models;
using MathTutor.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolClassController : BaseApiController
    {
        private readonly ISchoolClassService _schoolClassService;
        private readonly ILogger<SchoolClassController> _logger;

        public SchoolClassController(
            ISchoolClassService schoolClassService,
            ILogger<SchoolClassController> logger)
        {
            _schoolClassService = schoolClassService ?? throw new ArgumentNullException(nameof(schoolClassService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SchoolClassModel>), 200)]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                var classes = await _schoolClassService.GetAllClassesAsync();
                return Ok(classes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving school classes");
                return StatusCode(500, "An error occurred while retrieving school classes");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SchoolClassModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetClassById(int id)
        {
            try
            {
                var schoolClass = await _schoolClassService.GetClassByIdAsync(id);

                if (schoolClass == null)
                {
                    return NotFound($"School class with ID {id} not found");
                }

                return Ok(schoolClass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving school class with ID {ClassId}", id);
                return StatusCode(500, "An error occurred while retrieving the school class");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SchoolClassModel), 201)]
        public async Task<IActionResult> CreateClass([FromBody] SchoolClassModel schoolClassModel)
        {
            try
            {
                var createdClass = await _schoolClassService.CreateClassAsync(schoolClassModel);
                return CreatedAtAction(nameof(GetClassById), new { id = createdClass.Id }, createdClass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating school class");
                return StatusCode(500, "An error occurred while creating the school class");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SchoolClassModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] SchoolClassModel schoolClassModel)
        {
            try
            {
                if (id != schoolClassModel.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var updatedClass = await _schoolClassService.UpdateClassAsync(schoolClassModel);
                return Ok(updatedClass);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating school class with ID {ClassId}", id);
                return StatusCode(500, "An error occurred while updating the school class");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteClass(int id)
        {
            try
            {
                await _schoolClassService.DeleteClassAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting school class with ID {ClassId}", id);
                return StatusCode(500, "An error occurred while deleting the school class");
            }
        }
    }
} 