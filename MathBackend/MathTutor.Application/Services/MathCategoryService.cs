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
    public class MathCategoryService : IMathCategoryService
    {
        private readonly IMathCategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MathCategoryService> _logger;

        public MathCategoryService(
            IMathCategoryRepository categoryRepository,
            IMapper mapper,
            ILogger<MathCategoryService> logger)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MathCategoryModel>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                return _mapper.Map<IEnumerable<MathCategoryModel>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories");
                throw;
            }
        }

        public async Task<MathCategoryModel> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return null;
                }
                return _mapper.Map<MathCategoryModel>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<MathCategoryModel> CreateCategoryAsync(MathCategoryModel categoryModel)
        {
            try
            {
                var category = _mapper.Map<MathCategory>(categoryModel);
                var createdCategory = await _categoryRepository.CreateCategoryAsync(category);
                return _mapper.Map<MathCategoryModel>(createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category {CategoryName}", categoryModel.Name);
                throw;
            }
        }

        public async Task<bool> UpdateCategoryAsync(int id, MathCategoryModel categoryModel)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
                if (existingCategory == null)
                {
                    return false;
                }

                // Update properties
                existingCategory.Name = categoryModel.Name;
                existingCategory.Description = categoryModel.Description;

                return await _categoryRepository.UpdateCategoryAsync(existingCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                return await _categoryRepository.DeleteCategoryAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}", id);
                throw;
            }
        }
    }
} 