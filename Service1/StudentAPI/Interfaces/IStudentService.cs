using StudentAPI.Model;

namespace StudentAPI.Interfaces
{
    public interface IStudentService
    {
        Task<string> CreateCourse(Course course);
        Task<string> UpdateCourse(Course course);
        Task<string> DeleteCourse(int id);
        Task<Course> GetCourseById(int id);
        Task<List<Course>> GetAllCourses();
    }
}
