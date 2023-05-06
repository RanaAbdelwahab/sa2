using Microsoft.EntityFrameworkCore;
using StudentAPI.Interfaces;
using StudentAPI.Model;

namespace StudentAPI.Implementation
{
    public class StudentService : IStudentService
    {
        private readonly DBContext _context;
        public StudentService(DBContext context) => (_context) = (context);
        public async Task<string> CreateCourse(Course course)
        {
            if (course != null)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return "Added";
            }
            else 
               return "Not Added";
        }

        public async Task<string> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
           
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return "Deleted";
                
            }
            return "Not Found";
        }

        public async Task<List<Course>> GetAllCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetCourseById(int id)
        {

            var student = await _context.Courses.FindAsync(id);
            return student;
        }

        public async Task<string> UpdateCourse(Course course)
        {
            

            if (course != null)
            {

                var UpdatedCourse = _context.Courses.Attach(course);
                UpdatedCourse.State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return "Updeted";
            }
            return "Not Found";

        }
    }
}
