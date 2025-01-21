using Microsoft.EntityFrameworkCore;
using Server.DataAccess;
using Server.DataAccess.Model;
using Server.Services.Interfaces;
using Shared.DTO;

namespace Server.Services;

public class ProductService : IProductService
{
    private readonly Db _db;
    private readonly IConfiguration _configuration;

    public ProductService(Db db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task Add(ProductDTO productDto)
    {
        Product newProduct = new Product()
        {
            Title = productDto.Title,
            Description = productDto.Description
        };

        await _db.Products.AddAsync(newProduct);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductDTO>> Get()
    {
        IEnumerable<ProductDTO> products = await _db.Products
            .Select(product => DTOMapper.ToDto(product))
            .ToListAsync();

        return products;
    }

    public async Task<bool> IsExisting(int id)
    {
        return await _db.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<ProductDTO> Update(int id, ProductDTO productDto)
    {
        Product toUpdateProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);

        toUpdateProduct.Title = productDto.Title;
        toUpdateProduct.Description = productDto.Description;

        await _db.SaveChangesAsync();

        return DTOMapper.ToDto(toUpdateProduct);
    }
}
