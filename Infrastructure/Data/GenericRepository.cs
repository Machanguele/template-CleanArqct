using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext _context;
        private DbSet<T> _table;
            
        public GenericRepository(DataContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _table.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _table.FindAsync(id);
        }
        public async Task<T> GetFirst()
        {
            return await _table.FirstOrDefaultAsync();
        }
    
        public async void AddAsync(T obj)
        {
            await _table.AddAsync(obj);
        }
        
        public void Update(T obj)
        {
            _table.Attach(obj);

            _context.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(T obj)
        {
            _table.Remove(obj);
        }

        public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
        {
            T data = await ApplySpecification(spec).FirstOrDefaultAsync();
            return data;
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            List<T> data = await ApplySpecification(spec).ToListAsync();
            return data;
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_table.AsQueryable(), spec);
        }
    }
}