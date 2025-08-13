using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main()
    {
        try
        {
            string apiKeyPath = Path.Combine(AppContext.BaseDirectory, "apikey.txt");

            if (!File.Exists(apiKeyPath))
            {
                Console.WriteLine("apikey.txt dosyası bulunamadı!");
                return;
            }

            string apiKey = File.ReadAllText(apiKeyPath, Encoding.UTF8)
                                .Replace("\uFEFF", "") // BOM temizle
                                .Trim();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("API key boş veya sadece boşluklardan oluşuyor!");
                return;
            }

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            Console.WriteLine("Lütfen sorunuzu yazınız:");
            string prompt = Console.ReadLine();

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 500
            };

            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            string responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseString);
                var answer = result.GetProperty("choices")[0]
                                   .GetProperty("message")
                                   .GetProperty("content")
                                   .GetString();

                Console.WriteLine("\nOpenAI'nin cevabı:");
                Console.WriteLine(answer);
            }
            else
            {
                Console.WriteLine($"API isteği başarısız. StatusCode: {response.StatusCode}");
                Console.WriteLine(responseString);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        }
    }
}