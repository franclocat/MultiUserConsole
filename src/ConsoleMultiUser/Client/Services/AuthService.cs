using Client.Services.Interfaces;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services;

internal class AuthService : IAuthService
{
    private const string _ENDPOINT = "/Auth/";
    private readonly HttpClient _client;
    public AuthService(string baseUrl)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(baseUrl + _ENDPOINT);
    }

    public async Task<ServiceResult> Login(UserDTO userDto)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("Login", userDto);
        return new ServiceResult(response);
    }

    public async Task<ServiceResult> Register(UserDTO userDto)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("Register", userDto);
        return new ServiceResult(response);
    }
}
