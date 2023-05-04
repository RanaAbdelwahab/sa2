using Confluent.Kafka;
using Newtonsoft.Json;
using StudentAPI.Interfaces;
using StudentAPI.Model;

namespace StudentAPI.Implementations
{
    public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue>
    {
        private readonly IConsumer<TKey, TValue> consumer;
        private readonly CancellationTokenSource cancellationTokenSource;

        public KafkaConsumer()
        {
            string topicName = "Course";
            var config = new ConsumerConfig
            {
                GroupId = "my-consumer-group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            this.consumer = new ConsumerBuilder<TKey, TValue>(config).Build();
            this.consumer.Subscribe(topicName);
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public void Consume(Action<TValue> handler)
        {
            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(cancellationTokenSource.Token);
                    if (consumeResult.Value != null)
                    {
                        handler(consumeResult.Value);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // This exception is expected when the CancellationToken is canceled.
            }
        }

        public void Dispose()
        {
            consumer.Unsubscribe();
            cancellationTokenSource.Cancel();
            consumer.Close();
            consumer.Dispose();
        }
    }
}
