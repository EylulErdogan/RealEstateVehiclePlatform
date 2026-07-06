using RealEstateVehiclePlatform.Entities.Abstract;
using System.Linq.Expressions;

namespace RealEstateVehiclePlatform.DataAccess.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        List<T> GetAll();

        T GetById(int id);

        T GetByFilter(Expression<Func<T, bool>> filter);

        void Insert(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}