using Client.Services.Interfaces;
using Shared.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Client.Services;

internal class AuthService : IAuthService
{
    private const string _ENDPOINT = "/Auth/";
    private readonly HttpClient _client;
    private string _authToken = string.Empty;
    public AuthService(string baseUrl)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(baseUrl + _ENDPOINT);
    }

    public async Task<GenericServiceResult<UserDTO>> Login(UserDTO userDto)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("Login", userDto);
        return new GenericServiceResult<UserDTO>(response);
    }

    public async Task<GenericServiceResult<UserDTO>> Register(UserDTO userDto)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("Register", userDto);
        return new GenericServiceResult<UserDTO>(response);
    }

    public void SetAuthToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _authToken = token;
    }

    public async Task<ServiceResult> CheckAuthorization()
    {
        HttpResponseMessage response = await _client.GetAsync("Checkauthorization");
        return new ServiceResult(response);
    }
}
