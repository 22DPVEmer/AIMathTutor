using AutoMapper;
using MathTutor.API.Constants;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MathProblemAttemptController : BaseApiController
    {
        private readonly IMathProblemAttemptRepository _attemptRepository;
        private readonly IMapper _mapper;

        public MathProblemAttemptController(
            IMathProblemAttemptRepository attemptRepository,
            IMapper mapper)
        {
            _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<MathProblemAttemptModel>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUserAttempts()
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(MathProblemAttemptControllerConstants.ErrorMessages.UserNotAuthenticated);
                }

                var attempts = await _attemptRepository.GetAttemptsByUserIdAsync(userId);
                var attemptModels = _mapper.Map<IEnumerable<MathProblemAttemptModel>>(attempts);
                return Ok(attemptModels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("problem/{problemId}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<MathProblemAttemptModel>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAttemptsByProblem(int problemId)
        {
            try
            {
                var attempts = await _attemptRepository.GetAttemptsByProblemIdAsync(problemId);
                var attemptModels = _mapper.Map<IEnumerable<MathProblemAttemptModel>>(attempts);
                return Ok(attemptModels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(MathProblemAttemptModel), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAttemptById(int id)
        {
            try
            {
                var attempt = await _attemptRepository.GetAttemptByIdAsync(id);
                if (attempt == null)
                {
                    return NotFound(string.Format(MathProblemAttemptControllerConstants.ErrorMessages.AttemptNotFound, id));
                }

                // Check if the attempt belongs to the current user
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId) || attempt.UserId != userId)
                {
                    return Unauthorized(MathProblemAttemptControllerConstants.ErrorMessages.NotAuthorizedToViewAttempt);
                }

                var attemptModel = _mapper.Map<MathProblemAttemptModel>(attempt);
                return Ok(attemptModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 