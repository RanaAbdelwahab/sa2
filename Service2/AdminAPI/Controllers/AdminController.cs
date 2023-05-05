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
               
                var key = Guid.NewGuid().ToString();
                var value = JsonConvert.SerializeObject(payload);

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

                await producer.DeleteCourseAsync(key);
                return Ok();
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
                await producer.UpdateCourseAsync(key, newValue);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to update course: {ex.Message}");
            }
        }

    }
}
