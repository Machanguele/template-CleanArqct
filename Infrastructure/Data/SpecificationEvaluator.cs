using System.Linq;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;
            
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);
            
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            
            if (spec.OrderByDescendig != null)
                query = query.OrderByDescending(spec.OrderByDescendig);

            if (spec.IsPagingEnable)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }
            
            query = spec.Includes.Aggregate(query, (current, include) 
                => current.Include(include));
            
            return query;
        }
    }
}