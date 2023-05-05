using Microsoft.AspNetCore.Mvc;
using StudentAPI.Interfaces;
using StudentAPI.Model;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
         
        }
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var result = await _studentService.GetAllCourses();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Course>> GetCourseById(int id)
        {
            var result = await _studentService.GetCourseById(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

       
    }
}
