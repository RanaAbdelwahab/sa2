using AdminAPI.Interfaces;
using AdminAPI.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdminAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService<string, string> producer;

        public AdminController(IAdminService<string, string> producer)
        {
            this.producer = producer;
        }
        
        [HttpPost]
        public async Task<IActionResult> ProduceToKafka([FromBody] Course payload)
        {
            try
            {
                CourseDto course = new CourseDto
                {
                    Name = payload.Name,
                    Description = payload.Description,
                    Deleted = 0,
                    Updated = 0
                };

                var key = Guid.NewGuid().ToString();
                var value = JsonConvert.SerializeObject(course);

                var p = await producer.ProduceAsync("Course", key, value);

                return Ok(p);
            }
            catch (ProduceException<string, string> ex)
            {
                return StatusCode(500, $"Failed to produce message: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCourse(string key)
        {
            try
            {
                CourseDto course = new CourseDto
                {
                    Id = int.Parse(key),
                    Deleted = 1
                };
                var value = JsonConvert.SerializeObject(course);
                var  p = await producer.DeleteCourseAsync("Course", key, value);
                return Ok(p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to delete course: {ex.Message}");
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateCourse(string key, [FromBody] Course newValue)
        {
            try
            {
                CourseDto course = new CourseDto
                {
                    Id = int.Parse(key),
                    Name = newValue.Name,
                    Description = newValue.Description,
                    Updated = 1
                };
                var value = JsonConvert.SerializeObject(course);
                var p = await producer.UpdateCourseAsync("Course", key, value);
                return Ok(p);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to update course: {ex.Message}");
            }
        }

    }
}
