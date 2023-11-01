using Microsoft.EntityFrameworkCore;

namespace JobManager.Data.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        List<T> GetAll();
        Task<T> GetById(int id);
        Task Create(T entity);
        Task CreateRange(List<T> entity);
        void Update(T entity);
        Task Save();
    }
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly JobManagerDbContext _context;
        private readonly DbSet<T> _entities;

        public GenericRepository(JobManagerDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public List<T> GetAll() =>
            _entities.ToList();

        public async Task<T> GetById(int id) =>
             await _entities.FindAsync(id);

        public async Task Create(T entity) =>
         await _context.AddAsync(entity);

        public async Task CreateRange(List<T> entity)
        {
            await _context.AddRangeAsync(entity);
        }

        public void Update(T entity)
        {
            _entities.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task Save() =>
      await _context.SaveChangesAsync();
    }
}
