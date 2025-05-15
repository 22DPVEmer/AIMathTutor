using MathTutor.Core.Models;

namespace MathTutor.Application.Interfaces{
    public interface ISchoolClassService
    {
        Task<IEnumerable<SchoolClassModel>> GetAllClassesAsync();
        Task<SchoolClassModel?> GetClassByIdAsync(int id);
        Task<SchoolClassModel> CreateClassAsync(SchoolClassModel schoolClass);
        Task<SchoolClassModel> UpdateClassAsync(SchoolClassModel schoolClass);
        Task DeleteClassAsync(int id);
    } 
}

