using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using ChatbotTest.Models;

namespace ChatbotTest.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // === New Chat Endpoint ===
    [HttpPost("/api/chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Message))
            return BadRequest(new { response = "Empty message." });

        try
        {
            _httpClient.BaseAddress = new Uri("http://charliethechatbot_n8n_1:5678/");

            // Adjust "webhook/chat" to your actual n8n webhook URL path
            var n8nResponse = await _httpClient.PostAsJsonAsync("webhook/chat", new { message = req.Message });

            if (!n8nResponse.IsSuccessStatusCode)
            {
                _logger.LogError("n8n returned error status {StatusCode}", n8nResponse.StatusCode);
                return StatusCode((int)n8nResponse.StatusCode, new { response = "Error contacting n8n." });
            }

            var result = await n8nResponse.Content.ReadFromJsonAsync<dynamic>();
            return Ok(new { response = result?["response"] ?? "No reply from n8n." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending message to n8n");
            return StatusCode(500, new { response = "Internal server error." });
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; }
}
