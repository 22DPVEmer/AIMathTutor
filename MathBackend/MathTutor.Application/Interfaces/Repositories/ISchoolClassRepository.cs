using MathTutor.Core.Entities;

namespace MathTutor.Application.Interfaces{
    public interface ISchoolClassRepository
    {
        Task<IEnumerable<SchoolClass>> GetAllClassesAsync();
        Task<SchoolClass?> GetClassByIdAsync(int id);
        Task<SchoolClass> CreateClassAsync(SchoolClass schoolClass);
        Task<SchoolClass> UpdateClassAsync(SchoolClass schoolClass);
        Task DeleteClassAsync(int id);
        Task<int> GetTopicCountAsync(int schoolClassId);
    } 
}

