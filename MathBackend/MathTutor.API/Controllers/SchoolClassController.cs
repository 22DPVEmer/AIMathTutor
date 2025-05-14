using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MathTutor.Core.Models;
using MathTutor.Application.Interfaces;
using MathTutor.API.Constants;
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

        public SchoolClassController(
            ISchoolClassService schoolClassService)
        {
            _schoolClassService = schoolClassService ?? throw new ArgumentNullException(nameof(schoolClassService));
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
            catch (Exception)
            {
                return StatusCode(500, string.Format(SchoolClassControllerConstants.ErrorMessages.ServerError, "retrieving school classes"));
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
                    return NotFound(string.Format(SchoolClassControllerConstants.ErrorMessages.ClassNotFound, id));
                }

                return Ok(schoolClass);
            }
            catch (Exception)
            {
                return StatusCode(500, string.Format(SchoolClassControllerConstants.ErrorMessages.ServerError, "retrieving the school class"));
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
            catch (Exception)
            {
                return StatusCode(500, string.Format(SchoolClassControllerConstants.ErrorMessages.ServerError, "creating the school class"));
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
                    return BadRequest(SchoolClassControllerConstants.ErrorMessages.IdMismatch);
                }

                var updatedClass = await _schoolClassService.UpdateClassAsync(schoolClassModel);
                return Ok(updatedClass);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, string.Format(SchoolClassControllerConstants.ErrorMessages.ServerError, "updating the school class"));
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
            catch (Exception)
            {
                return StatusCode(500, string.Format(SchoolClassControllerConstants.ErrorMessages.ServerError, "deleting the school class"));
            }
        }
    }
} 