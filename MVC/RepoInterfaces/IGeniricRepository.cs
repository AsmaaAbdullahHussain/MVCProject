using System.Linq.Expressions;

namespace mvc.RepoInterfaces
{
    public interface IGeniricRepository<Id, T> where T : class
    {
        Task AddAsync(T entity);
        Task<T> GetByIdAsync(Id id);
        IQueryable<T> GetAll(int? pageNumber, int? size);
        IQueryable<T> Search(Expression<Func<T, bool>> predicate, int? pageNumber, int? size);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate);
        void Update(T updated);
        Task DeleteAsync(Id id);//
        Task<int> SaveAsync();
    }
}
