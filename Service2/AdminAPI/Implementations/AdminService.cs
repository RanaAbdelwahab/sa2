using AdminAPI.Interfaces;
using AdminAPI.Models;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdminAPI.Implementations
{
    public class AdminService<TKey, TValue> : IAdminService<TKey, TValue>
    {
            private readonly ProducerConfig config;
            private readonly IProducer<TKey, TValue> producer;
            private readonly AdminClientConfig _config;

            public AdminService()
            {
                this.config = new ProducerConfig { BootstrapServers = "kafka:29092" };
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
                var result = $"Course Added (value: {JsonConvert.SerializeObject(value)}, partition: {deliveryResult.Partition}, offset: {deliveryResult.Offset})";
                Console.WriteLine(result);
                return result;
            }

        public async Task<string> DeleteCourseAsync(string topicName, TKey key, TValue value)
        {
            var message = new Message<TKey, TValue>
            {
                Key = key,
                Value = value
            };

            var deliveryResult = await producer.ProduceAsync(topicName, message);
            var result = $"Course Deleted (value: {value}, partition: {deliveryResult.Partition}, offset: {deliveryResult.Offset})";
            Console.WriteLine(result);
            return result;
        }

        public async Task<string> UpdateCourseAsync(string topicName, TKey key, TValue value)
        {
            var message = new Message<TKey, TValue>
            {
                Key = key,
                Value = value
            };

            var deliveryResult = await producer.ProduceAsync(topicName, message);
            var result = $"Course Updated (value: {value}, partition: {deliveryResult.Partition}, offset: {deliveryResult.Offset})";
            Console.WriteLine(result);
            return result;
        }
        public void Dispose()
            {
                producer.Flush();
                producer.Dispose();
            }
        }
    }

