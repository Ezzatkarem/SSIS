using SSIS.Domain.Entities;

namespace SSIS.Domain.Interfaces
{
    public interface ICoursePrerequisiteRepository : IRepository<CoursePrerequesite>
    {
        Task<List<Guid>> GetPrerequisiteIdsByCourseAsync(Guid courseId);
        Task<List<string>> GetNamesByIdsAsync(List<Guid> courseIds);
    }
}