using MathTutor.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IMathCategoryService
    {
        Task<IEnumerable<MathCategoryModel>> GetAllCategoriesAsync();
        Task<MathCategoryModel> GetCategoryByIdAsync(int id);
        Task<MathCategoryModel> CreateCategoryAsync(MathCategoryModel categoryModel);
        Task<bool> UpdateCategoryAsync(int id, MathCategoryModel categoryModel);
        Task<bool> DeleteCategoryAsync(int id);
    }
}