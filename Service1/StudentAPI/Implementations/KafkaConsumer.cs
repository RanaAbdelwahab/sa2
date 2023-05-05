using Confluent.Kafka;
using Newtonsoft.Json;
using StudentAPI.Interfaces;
using StudentAPI.Model;

namespace StudentAPI.Implementations
{
    public class KafkaConsumer : IHostedService, IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private Task _consumeTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly IStudentService _studentService;

        public KafkaConsumer(IStudentService studentService)
        {
            _studentService = studentService;
            var bootstrapServers = "localhost:9092";
            var groupId = "my-consumer-group";
            var topic = "Course";

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _consumer.Subscribe(topic);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumeTask = Task.Run(() => ConsumeAsync(_stoppingCts.Token));
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _stoppingCts.Cancel();

            try
            {
                await _consumeTask;
            }
            catch (TaskCanceledException)
            {
                // This is expected
            }
            finally
            {
                _consumer.Close();
                _consumer.Dispose();
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
            _consumer.Close();
            _consumer.Dispose();
        }

        private async Task ConsumeAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    var course = JsonConvert.DeserializeObject<CourseDto>(consumeResult.Message.Value);
                    Course newCourse = new Course
                    {
                        Id = (int)consumeResult.TopicPartitionOffset.Offset,
                        Name = course.Name,
                        Description = course.Description

                    };
                    Console.WriteLine($"Received message: {consumeResult.Message.Value}");

                    if (course.Deleted == 1)
                    {
                        //var result = await _studentService.DeleteCourse();
                        //Console.WriteLine($"Message Deleted: {result}");
                    }
                    else if (course.Updated == 1)
                    {
                        var result = await _studentService.UpdateCourse(newCourse);
                        Console.WriteLine($"Message Updated: {result}");
                    }
                    else
                    {
                        var result = await _studentService.CreateCourse(newCourse);
                        Console.WriteLine($"Message Added: {result}");
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while consuming message from Kafka: {ex.Message}");
                }
            }
        }

    }
}
