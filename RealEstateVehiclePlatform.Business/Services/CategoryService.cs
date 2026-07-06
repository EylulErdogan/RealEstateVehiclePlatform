using RealEstateVehiclePlatform.Business.Interfaces;
using RealEstateVehiclePlatform.DataAccess.Interfaces;
using RealEstateVehiclePlatform.Entities.Concrete;

namespace RealEstateVehiclePlatform.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Category> GetAll()
        {
            return _unitOfWork.Categories.GetAll();
        }

        public Category GetById(int id)
        {
            return _unitOfWork.Categories.GetById(id);
        }

        public void Create(Category category)
        {
            var existingCategory = _unitOfWork.Categories
                .GetByFilter(x => x.Name == category.Name && !x.IsDeleted);

            if (existingCategory != null)
            {
                throw new Exception("Bu kategori zaten mevcut.");
            }

            _unitOfWork.Categories.Insert(category);
            _unitOfWork.Save();
        }

        public void Update(Category category)
        {
            var value = _unitOfWork.Categories.GetById(category.Id);

            if (value == null)
            {
                throw new Exception("Kategori bulunamadı.");
            }

            value.Name = category.Name;

            _unitOfWork.Categories.Update(value);
            _unitOfWork.Save();
        }

        public void Delete(int id)
        {
            var value = _unitOfWork.Categories.GetById(id);

            if (value == null)
            {
                throw new Exception("Kategori bulunamadı.");
            }

            _unitOfWork.Categories.Delete(value);
            _unitOfWork.Save();
        }
    }
}