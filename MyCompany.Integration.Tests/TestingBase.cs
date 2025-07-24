using System.Text;
using System.Text.Json;

namespace MyCompany.Integration.Tests;

public class TestingBase
{
    private readonly HttpClient _httpClient;
    protected const string ApiRoot = "http://localhost:5054";

    protected TestingBase()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiRoot)
        };
    }

    /// <summary>
    /// Sends a POST request to the specified API path with the given request DTO.
    /// </summary>
    protected async Task<HttpResponseMessage> DoPostRequest(string apiPath, object requestDto)
    {
        var requestBody = new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");
        var apiResponse = await _httpClient.PostAsync(apiPath, requestBody);

        return apiResponse;
    }

    /// <summary>
    /// Sends a GET request to the specified API path.
    /// </summary>
    protected async Task<HttpResponseMessage> DoGetRequest(string apiPath)
    {
        var apiResponse = await _httpClient.GetAsync(apiPath);
        return apiResponse;
    }

    /// <summary>
    /// Sends a PUT request to the specified API path with the given request DTO.
    /// </summary>
    protected async Task<HttpResponseMessage> DoPatchRequest(string apiPath, object requestDto)
    {
        var requestBody = new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");
        var apiResponse = await _httpClient.PatchAsync(apiPath, requestBody);

        return apiResponse;
    }

    /// <summary>
    /// Sends a DELETE request to the specified API path.
    /// </summary>
    protected async Task<HttpResponseMessage> DoDeleteRequest(string apiPath)
    {
        var apiResponse = await _httpClient.DeleteAsync(apiPath);
        return apiResponse;
    }
}