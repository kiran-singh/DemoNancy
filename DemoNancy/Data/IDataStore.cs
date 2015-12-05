using System.Collections.Generic;
using DemoNancy.Model;

namespace DemoNancy.Data
{
    public interface IDataStore<T> where T : IData
    {
        bool Add(T todo);
        void Clear();
        int Count { get; }
        IEnumerable<T> GetAll();
        bool Remove(int id);
        bool Update(T todo);
    }
}