using StudentAPI.Model;

namespace StudentAPI.Interfaces
{
    public interface IStudentService
    {
        Task<Course> CreateCourse(Course course);
        Task<Course> UpdateCourse(Course course);
        Task DeleteCourse(int id);
        Task<Course> GetCourseById(int id);
        Task<List<Course>> GetAllCourses();
    }
}
