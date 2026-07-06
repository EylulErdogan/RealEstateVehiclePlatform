using Microsoft.EntityFrameworkCore;
using RealEstateVehiclePlatform.DataAccess.Context;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Abstract;
using System.Linq.Expressions;

namespace RealEstateVehiclePlatform.DataAccess.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public List<T> GetAll()
        {
            return _dbSet.Where(x => !x.IsDeleted).ToList();
        }

        public T GetById(int id)
        {
            return _dbSet.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public T GetByFilter(Expression<Func<T, bool>> filter)
        {
            return _dbSet.FirstOrDefault(filter);
        }

        public void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.UpdatedDate = DateTime.Now;
            _dbSet.Update(entity);
        }
    }
}