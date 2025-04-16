namespace App.Repositories;

public interface IProductRepository: IGenericRepository<Product>
{
    public Task<List<Product>> GetTopPriceProductsAsync(int count);
}