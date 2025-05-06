using AutoMapper;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    public class SchoolClassService : ISchoolClassService
    {
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SchoolClassService> _logger;

        public SchoolClassService(
            ISchoolClassRepository schoolClassRepository,
            IMapper mapper,
            ILogger<SchoolClassService> logger)
        {
            _schoolClassRepository = schoolClassRepository ?? throw new ArgumentNullException(nameof(schoolClassRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<SchoolClassModel>> GetAllClassesAsync()
        {
            try
            {
                var schoolClasses = await _schoolClassRepository.GetAllClassesAsync();
                return _mapper.Map<IEnumerable<SchoolClassModel>>(schoolClasses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all school classes");
                throw;
            }
        }

        public async Task<SchoolClassModel?> GetClassByIdAsync(int id)
        {
            try
            {
                var schoolClass = await _schoolClassRepository.GetClassByIdAsync(id);
                if (schoolClass == null)
                {
                    return null;
                }
                return _mapper.Map<SchoolClassModel>(schoolClass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving school class with ID {SchoolClassId}", id);
                throw;
            }
        }

        public async Task<SchoolClassModel> CreateClassAsync(SchoolClassModel schoolClassModel)
        {
            try
            {
                var schoolClass = _mapper.Map<SchoolClass>(schoolClassModel);
                var createdSchoolClass = await _schoolClassRepository.CreateClassAsync(schoolClass);
                return _mapper.Map<SchoolClassModel>(createdSchoolClass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating school class {SchoolClassName}", schoolClassModel.Name);
                throw;
            }
        }

        public async Task<SchoolClassModel> UpdateClassAsync(SchoolClassModel schoolClassModel)
        {
            try
            {
                var existingSchoolClass = await _schoolClassRepository.GetClassByIdAsync(schoolClassModel.Id);
                if (existingSchoolClass == null)
                {
                    throw new ArgumentException($"School class with ID {schoolClassModel.Id} not found");
                }

                // Update properties
                existingSchoolClass.Name = schoolClassModel.Name;
                existingSchoolClass.Description = schoolClassModel.Description;

                var updatedSchoolClass = await _schoolClassRepository.UpdateClassAsync(existingSchoolClass);
                return _mapper.Map<SchoolClassModel>(updatedSchoolClass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating school class with ID {SchoolClassId}", schoolClassModel.Id);
                throw;
            }
        }

        public async Task DeleteClassAsync(int id)
        {
            try
            {
                await _schoolClassRepository.DeleteClassAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting school class with ID {SchoolClassId}", id);
                throw;
            }
        }
    }
} 