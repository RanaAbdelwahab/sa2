using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using StudentAPI.Interfaces;
using StudentAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        private readonly IKafkaConsumer<string, string> _kafkaConsumer;
        private readonly CancellationTokenSource cancellationTokenSource;
        public StudentController(IStudentService studentService, IKafkaConsumer<string, string> kafkaConsumer)
        {
            _studentService = studentService;
            _kafkaConsumer = kafkaConsumer;
            this.cancellationTokenSource = new CancellationTokenSource();


        }
        [HttpGet]
        public async Task<IEnumerable<string>> GetKafkaMessages()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "my-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var messages = new List<string>();

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("Course");

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var result = await Task.Run(() => consumer.Consume());
                    messages.Add(result.Value);
                }
            }

            return messages;
        }
        [HttpGet]
        public async Task<IActionResult> StartConsuming()
        {
            var result="";
            await Task.Run(() =>
            {
                _kafkaConsumer.Consume(value =>
                {
                    result = value;
                    Console.WriteLine($"Received message: {value}");
                });
                
            });

            return Ok(result);
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

        [HttpPost]
        public async Task<ActionResult<Course>> AddCourse([FromBody] CourseDto course)
        {
            var newCourse = new Course()
            {
                Name = course.Name,
                Description = course.Description,
            };

            var result = await _studentService.CreateCourse(newCourse);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            await _studentService.DeleteCourse(id);
            return Ok();

        }

        [HttpPut]
        public async Task<IActionResult> UpdateCourse([FromBody] Course course)
        {

            var result = await _studentService.UpdateCourse(course);
            return Ok(result);

        }
    }
}
