using MathTutor.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Core.Interfaces.Services
{
    public interface IMathCategoryService
    {
        Task<IEnumerable<MathCategoryModel>> GetAllCategoriesAsync();
        Task<MathCategoryModel> GetCategoryByIdAsync(int id);
        Task<MathCategoryModel> CreateCategoryAsync(MathCategoryModel category);
        Task<bool> UpdateCategoryAsync(MathCategoryModel category);
        Task<bool> DeleteCategoryAsync(int id);
    }
} 