using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

public class ApiAuthService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TokenSessionKey = "ApiToken";

    public ApiAuthService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _clientFactory = clientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var client = _clientFactory.CreateClient("ReportingApi");

        var loginBody = new { Email = email, Password = password };

        var response = await client.PostAsJsonAsync("api/auth/login", loginBody);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        using var contentStream = await response.Content.ReadAsStreamAsync();
        var json = await JsonSerializer.DeserializeAsync<JsonElement>(contentStream);

        var token = json.GetProperty("token").GetString();

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null && !string.IsNullOrEmpty(token))
        {
            httpContext.Session.SetString(TokenSessionKey, token);
        }

        return true;
    }

    public HttpClient GetAuthorizedClient()
    {
        var client = _clientFactory.CreateClient("ReportingApi");

        var httpContext = _httpContextAccessor.HttpContext;
        var token = httpContext?.Session.GetString(TokenSessionKey);

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public bool HasToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var token = httpContext?.Session.GetString(TokenSessionKey);
        return !string.IsNullOrEmpty(token);
    }
    public void Logout()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext?.Session.Remove(TokenSessionKey);
    }
}