using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MathTopicController : BaseApiController
    {
        private readonly IMathTopicService _mathTopicService;

        public MathTopicController(IMathTopicService mathTopicService)
        {
            _mathTopicService = mathTopicService ?? throw new ArgumentNullException(nameof(mathTopicService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MathTopicModel>), 200)]
        public async Task<IActionResult> GetAllTopics()
        {
            var topics = await _mathTopicService.GetAllTopicsAsync();
            return HandleResult(topics);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MathTopicModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTopicById(int id)
        {
            var topic = await _mathTopicService.GetTopicByIdAsync(id);
            return HandleResult(topic);
        }

        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(typeof(IEnumerable<MathTopicModel>), 200)]
        public async Task<IActionResult> GetTopicsByCategory(int categoryId)
        {
            var topics = await _mathTopicService.GetTopicsByCategoryAsync(categoryId);
            return HandleResult(topics);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(typeof(MathTopicModel), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTopic([FromBody] MathTopicModel model)
        {
            try
            {
                var result = await _mathTopicService.CreateTopicAsync(model);
                if (result == null)
                {
                    return BadRequest("Failed to create topic");
                }
                
                return CreatedAtAction(nameof(GetTopicById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTopic(int id, [FromBody] MathTopicModel model)
        {
            try
            {
                var topic = await _mathTopicService.GetTopicByIdAsync(id);
                if (topic == null)
                {
                    return NotFound();
                }
                
                // Ensure the ID is set correctly
                model.Id = id;
                
                var result = await _mathTopicService.UpdateTopicAsync(id, model);
                if (!result)
                {
                    return BadRequest("Failed to update topic");
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            try
            {
                var topic = await _mathTopicService.GetTopicByIdAsync(id);
                if (topic == null)
                {
                    return NotFound();
                }
                
                var result = await _mathTopicService.DeleteTopicAsync(id);
                if (!result)
                {
                    return BadRequest("Failed to delete topic");
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 