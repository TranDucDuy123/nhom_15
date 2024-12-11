using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyAspNetApp.Controllers
{
    public class SendDataController : Controller
    {
        private readonly HttpClient _httpClient;
        public SendDataController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: SendData/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: SendData/PostData
        [HttpPost]
        public async Task<IActionResult> PostData(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                ViewBag.ErrorMessage = "Plaintext cannot be empty.";
                return View("Index");
            }

            // Prepare the data to send to the HybridEncryptionController
            var content = new StringContent(JsonConvert.SerializeObject(new { plaintext = plaintext }), Encoding.UTF8, "application/json");

            // Send POST request to the HybridEncryptionController's Encrypt action
            var response = await _httpClient.PostAsync("http://localhost:5013/HybridEncryption/Encrypt", content);

            if (response.IsSuccessStatusCode)
            {
                // Parse the JSON response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var encryptionResult = JsonConvert.DeserializeObject<EncryptionResult>(jsonResponse);

                // Pass the result to the view
                ViewBag.EncryptedText = encryptionResult.EncryptedText;
                ViewBag.EncryptedKey = encryptionResult.EncryptedKey;
                ViewBag.EncryptedIV = encryptionResult.EncryptedIV;
                ViewBag.Plaintext = plaintext;
            }
            else
            {
                ViewBag.ErrorMessage = "Encryption failed.";
            }

            return View("Index");
        }
    }

    // Helper class to deserialize the response
    public class EncryptionResult
    {
        public string EncryptedText { get; set; }
        public string EncryptedKey { get; set; }
        public string EncryptedIV { get; set; }
    }
}
