using MathTutor.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Application.Interfaces
{
    public interface IMathCategoryRepository
    {
        Task<IEnumerable<MathCategory>> GetAllCategoriesAsync();
        Task<MathCategory> GetCategoryByIdAsync(int id);
        Task<MathCategory> CreateCategoryAsync(MathCategory category);
        Task<bool> UpdateCategoryAsync(MathCategory category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}