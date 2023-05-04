using Newtonsoft.Json.Linq;
using StudentAPI.Model;

namespace StudentAPI.Interfaces
{
    public interface IKafkaConsumer<TKey, TValue> : IDisposable
    {
        void Consume(Action<TValue> handler);
    }
}
