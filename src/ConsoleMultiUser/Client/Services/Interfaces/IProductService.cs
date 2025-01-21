using Shared.DTO;

namespace Client.Services.Interfaces;

interface IProductService
{
    void SetAuthToken(string token);
    Task<GenericServiceResult<IEnumerable<ProductDTO>>> GetAll();
    Task<GenericServiceResult<ProductDTO>> Update(ProductDTO productDto);
    Task<ServiceResult> Add(ProductDTO productDto);
}
