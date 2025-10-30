using System.Diagnostics;
using System.Text;
using System.Web;
using Data.Models;
using Data.Models.Dto;
using Data.Repositories;

namespace Core.Services;

public class GreatRequestExecutor
{
    private readonly HttpClient _httpClient = new(); // TODO: IHttpClientFactory
    private readonly HistoryDbRepository _historyDbRepository;

    public GreatRequestExecutor(HistoryDbRepository historyDbRepository)
    {
        _historyDbRepository = historyDbRepository;
    }

    public async Task<ApiResponse> ExecuteRequestAsync(Request request)
    {
        var startTime = Stopwatch.StartNew();
        HttpResponseMessage? httpResponse = null;
        string? responseBody = null;
        
        try
        {
            var fullUrl = BuildUrlWithQuery(request.Url, request.Query);
            
            var method = new HttpMethod(request.Method.ToUpperInvariant());
            var message = new HttpRequestMessage(method, fullUrl);
            
            ConfigureHeadersAndContent(message, request);
            
            httpResponse = await _httpClient.SendAsync(message);


            responseBody = await httpResponse.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            return CreateErrorResponse(startTime, ex.Message);
        }
        finally
        {
            startTime.Stop();
        }

        var response = await CreateApiResponse(startTime.Elapsed, httpResponse!, responseBody);
        _ = Task.Run(() => _historyDbRepository.Insert(new HistoryEntry
        {
            Request = request,
            Response = response,
        }));
        
        return response;
    }
    
    private string BuildUrlWithQuery(string baseUrl, Dictionary<string, string> queryParams)
    {
        if (queryParams.Count == 0) return baseUrl;
        
        var builder = new UriBuilder(baseUrl);
        var query = HttpUtility.ParseQueryString(builder.Query);

        foreach (var (key, value) in queryParams)
        {
            query[key] = value;
        }

        builder.Query = query.ToString();
        return builder.ToString();
    }
    
    private void ConfigureHeadersAndContent(HttpRequestMessage message, Request request)
    {
        var hasBody = !string.IsNullOrEmpty(request.Body);

        if (hasBody)
        {
            message.Content = new StringContent(
                request.Body!, 
                Encoding.UTF8, 
                request.BodyType ?? "application/json"
            );
        }
        
        foreach (var header in request.Headers)
        {
            if (!message.Headers.TryAddWithoutValidation(header.Key, header.Value))
            {
                message.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }
    }
    
    private async Task<ApiResponse> CreateApiResponse(TimeSpan elapsed, HttpResponseMessage response, string? body)
    {
        var headers = new Dictionary<string, string>();
        
        foreach (var header in response.Headers.Concat(response.Content.Headers))
        {
            headers[header.Key] = string.Join(", ", header.Value);
        }
        
        var size = (int)(response.Content.Headers.ContentLength ?? 
                         (body != null ? Encoding.UTF8.GetBytes(body).Length : 0));

        return new ApiResponse
        {
            StatusCode = (int)response.StatusCode,
            Body = body,
            BodyType = response.Content.Headers.ContentType?.MediaType,
            Headers = headers,
            ResponseTime = elapsed,
            Size = size
        };
    }
    
    private ApiResponse CreateErrorResponse(Stopwatch stopwatch, string errorMessage)
    {
        stopwatch.Stop();
        return new ApiResponse
        {
            StatusCode = 0,
            Body = $"Network Error: {errorMessage}",
            BodyType = "text/plain",
            ResponseTime = stopwatch.Elapsed
        };
    }
}