using Client.Services.Interfaces;
using Shared.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Client.Services;

internal class ProductService : IProductService
{
    private const string _ENDPOINT = "/Product/";
    private readonly HttpClient _client;
    private string _authToken = string.Empty;
    public ProductService(string baseUrl)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(baseUrl + _ENDPOINT);
    }

    public async Task<ServiceResult> Add(ProductDTO productDto)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("", productDto);
        return new ServiceResult(response);
    }

    public async Task<GenericServiceResult<IEnumerable<ProductDTO>>> GetAll()
    {
        HttpResponseMessage response = await _client.GetAsync("");
        return new GenericServiceResult<IEnumerable<ProductDTO>>(response);
    }

    public void SetAuthToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _authToken = token;
    }

    public async Task<GenericServiceResult<ProductDTO>> Update(ProductDTO productDto)
    {
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{productDto.Id}", productDto);
        return new GenericServiceResult<ProductDTO>(response);
    }
}
