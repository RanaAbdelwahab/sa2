
using AdminAPI.Models;

namespace AdminAPI.Interfaces
{
    public interface IAdminService<TKey, TValue> : IDisposable
    {
        Task<string> ProduceAsync(string topicName, TKey key, TValue value);
        Task<string> DeleteCourseAsync(string topicName, TKey key, TValue value);
        Task<string> UpdateCourseAsync(string topicName, TKey key, TValue value);
    }
}
