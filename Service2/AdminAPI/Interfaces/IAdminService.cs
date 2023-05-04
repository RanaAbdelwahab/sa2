
using AdminAPI.Models;

namespace AdminAPI.Interfaces
{
    public interface IAdminService<TKey, TValue> : IDisposable
    {
        Task<string> ProduceAsync(string topicName, TKey key, TValue value);
        Task DeleteCourseAsync(string key);
        Task UpdateCourseAsync(string key, Course newValue);
    }
}
