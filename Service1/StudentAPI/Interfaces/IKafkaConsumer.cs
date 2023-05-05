using Newtonsoft.Json.Linq;
using StudentAPI.Model;

namespace StudentAPI.Interfaces
{
    public interface IKafkaConsumer<TKey, TValue> : IDisposable
    {
        public List<Course> Consume();
        Course ConsumeById(int id);
    }
}
