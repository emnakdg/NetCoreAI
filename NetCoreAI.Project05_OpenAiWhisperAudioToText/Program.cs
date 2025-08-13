using System.Net.Http.Headers;
using System.Text;

class Program
{
    static async Task Main(string[] args)
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

            string audioFilePath = "AnadoluKurtlari.mp3";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var form = new MultipartFormDataContent();

                var audioContent = new ByteArrayContent(File.ReadAllBytes(audioFilePath));
                audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg");
                form.Add(audioContent, "file", Path.GetFileName(audioFilePath));
                form.Add(new StringContent("whisper-1"), "model");

                Console.WriteLine("Ses dosyası işleniyor, Lütfen bekleyiniz...");

                var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("İşlem başarılı, Transkript:");
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine($"Hata: {response.StatusCode}");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        }
    }
}