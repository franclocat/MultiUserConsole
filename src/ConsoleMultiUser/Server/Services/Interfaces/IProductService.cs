using Shared.DTO;

namespace Server.Services.Interfaces;

public interface IProductService
{
    Task<bool> IsExisting(int id);
    Task<ProductDTO> Update(int id, ProductDTO productDto);
    Task<IEnumerable<ProductDTO>> Get();
    Task Add(ProductDTO productDto);
}
