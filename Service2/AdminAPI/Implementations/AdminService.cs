using AdminAPI.Interfaces;
using AdminAPI.Models;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;

namespace AdminAPI.Implementations
{
    public class AdminService<TKey, TValue> : IAdminService<TKey, TValue>
    {
            private readonly ProducerConfig config;
            private readonly IProducer<TKey, TValue> producer;
            private readonly AdminClientConfig _config;

            public AdminService()
            {
                this.config = new ProducerConfig { BootstrapServers = "localhost:9092" };
                this.producer = new ProducerBuilder<TKey, TValue>(config).Build();
               //this._config = new AdminClientConfig { BootstrapServers = "localhost:9092" };
            
        }

            public async Task<string> ProduceAsync(string topicName, TKey key, TValue value)
            {
                var message = new Message<TKey, TValue>
                {
                    Key = key,
                    Value = value
                };

                var deliveryResult = await producer.ProduceAsync(topicName, message);
                var result = $"Message sent (value: {JsonConvert.SerializeObject(value)}, partition: {deliveryResult.Partition}, offset: {deliveryResult.Offset})";
                Console.WriteLine($"Message sent (value: {JsonConvert.SerializeObject(value)}, partition: {deliveryResult.Partition}, offset: {deliveryResult.Offset})");
                return result;
            }

        public async Task DeleteCourseAsync(string key)
        {
            using (var adminClient = new AdminClientBuilder(_config).Build())
            {
                var metadata = adminClient.GetMetadata("Course", TimeSpan.FromSeconds(5));
                var partition = metadata.Topics.SelectMany(t => t.Partitions).FirstOrDefault(p => p.Leader != null && p.PartitionId == Convert.ToInt32(key) % metadata.Topics.Count);

                if (partition != null)
                {
                    var topicPartition = new TopicPartition("Course", new Partition(partition.PartitionId));
                    var offset = new Offset(Convert.ToInt64(key));
                    await adminClient.DeleteRecordsAsync(new[] { new TopicPartitionOffset(topicPartition, offset) });
                }
                else
                {
                    throw new Exception($"Partition not found for key {key}");
                }
            }
        }

        public async Task UpdateCourseAsync(string key, Course updatedCourse)
        {
            var updatedCourseJson = JsonConvert.SerializeObject(updatedCourse);
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var message = new Message<Null, string>
                {
                    Key = null,
                    Value = updatedCourseJson,
                    Timestamp = new Timestamp(DateTime.UtcNow)
                };
                await producer.ProduceAsync("Course", new Message<Null, string> { Key = null, Value = updatedCourseJson });
            }
        }
        public void Dispose()
            {
                producer.Flush();
                producer.Dispose();
            }
        }
    }

