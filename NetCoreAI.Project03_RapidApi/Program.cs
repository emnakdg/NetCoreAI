using NetCoreAI.Project03_RapidApi.ViewModels;
using Newtonsoft.Json;

var client = new HttpClient();
List<ApiSeriesViewModel> apiSeriesViewModels = new List<ApiSeriesViewModel>();
var request = new HttpRequestMessage
{
    Method = HttpMethod.Get,
    RequestUri = new Uri("https://imdb-top-100-movies.p.rapidapi.com/series/"),
    Headers =
    {
        { "x-rapidapi-key", "da631e8d35msh68d38af11234258p1aac95jsn15057c9e01cd" },
        { "x-rapidapi-host", "imdb-top-100-movies.p.rapidapi.com" },
    },
};
using (var response = await client.SendAsync(request))
{
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStringAsync();
    apiSeriesViewModels = JsonConvert.DeserializeObject<List<ApiSeriesViewModel>>(body);
    foreach (var series in apiSeriesViewModels)
    {
        Console.WriteLine($"Rank: {series.rank}, Title: {series.title}, Rating: {series.rating}, Year: {series.year}");
    }
}

Console.ReadLine();