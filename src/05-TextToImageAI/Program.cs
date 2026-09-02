//using Google.GenAI.Types;
//using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string? hfToken = config["HuggingFaceApiKey"];

string modelId = "black-forest-labs/FLUX.1-schnell";

string prompt = "A wide cinematic shot of a colossal, sprawling orbital space station hovering in deep space. " +
                "Titanium hulls and glowing interior docking bays show massive cargo spacecraft moored to docking arms. " +
                "Intricate exterior gantry cranes, solar array grids, and navigation lights illuminate the metallic surfaces. " +
                "The background features the vivid blue curve of a planet catching sunlight, harsh directional studio lighting, " +
                "deep space shadows, shot on an IMAX 70mm lens with extreme depth and detail.";


using var client = new HttpClient();

var request = new HttpRequestMessage(HttpMethod.Post, "https://router.huggingface.co/nscale/v1/images/generations");
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", hfToken);

var requestBody = new
{
    prompt,
    model = "black-forest-labs/FLUX.1-schnell",
    n = 1,
    size = "1024x1024"
};

request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

HttpResponseMessage response = await client.SendAsync(request);

if (response.IsSuccessStatusCode)
{
    string responseJson = await response.Content.ReadAsStringAsync();
    using var doc = JsonDocument.Parse(responseJson);

    // Parse OpenAI-formatted JSON output: { "data": [ { "b64_json": "..." } ] }
    var dataElement = doc.RootElement.GetProperty("data")[0];

    if (dataElement.TryGetProperty("b64_json", out var b64Element))
    {
        byte[] imageBytes = Convert.FromBase64String(b64Element.GetString()!);
        await File.WriteAllBytesAsync("space_station_csharp.png", imageBytes);
        Console.WriteLine("Success! Image saved to space_station_csharp.png");
    }
    else if (dataElement.TryGetProperty("url", out var urlElement))
    {
        byte[] imageBytes = await client.GetByteArrayAsync(urlElement.GetString()!);
        await File.WriteAllBytesAsync("space_station_csharp.png", imageBytes);
        Console.WriteLine("Success! Image downloaded and saved to space_station_csharp.png");
    }
}
else
{
    string error = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"API Error ({response.StatusCode}): {error}");
}


//var requestBody = new
//{
//    inputs = prompt,
//    model = modelId,
//    n = 1,
//    size = "1024x1024"
//};

//string json = JsonSerializer.Serialize(requestBody);
//var content = new StringContent(json, Encoding.UTF8, "application/json");

//string requestUrl = "https://router.huggingface.co/nscale/v1/images/generations";

//// Send request to Hugging Face Serverless Inference API
//HttpResponseMessage response = await client.PostAsync(requestUrl, content);

//if (response.IsSuccessStatusCode)
//{
//    string responseJson = await response.Content.ReadAsStringAsync();
//    using var doc = JsonDocument.Parse(responseJson);

//    // The endpoint returns JSON structured as: { "data": [ { "b64_json": "..." } ] }
//    var dataElement = doc.RootElement.GetProperty("data")[0];

//    if (dataElement.TryGetProperty("b64_json", out var b64Element))
//    {
//        byte[] imageBytes = Convert.FromBase64String(b64Element.GetString()!);
//        await File.WriteAllBytesAsync("space_station_csharp.png", imageBytes);
//        Console.WriteLine("Saved image to space_station_csharp.png!");
//    }
//}
//else
//{
//    string error = await response.Content.ReadAsStringAsync();
//    Console.WriteLine($"Error ({response.StatusCode}): {error}");
//}

//string? model = "imagen-4-fast-generate";
//string? model = "imagen-4.0-generate-001";
//string? model = "gemini-3.1-flash-lite-image";

//var client = new Client(apiKey: key);

//// Retrieve all supported models
//var models = await client.Models.ListAsync();

//await foreach (var mod in models)
//{
//    // Filter for image generation models
//    if (mod.Name.Contains("imagen") || mod.Name.Contains("image"))
//    {
//        // Output format: models/imagen-4.0-generate-001
//        Console.WriteLine($"Display Name: {mod.DisplayName} {mod.Version} {mod}");
//        Console.WriteLine($"API Model Name: {mod.Name}\n");
//    }
//}

//    var generator = new Client(apiKey: key)
//    .AsIImageGenerator(model);

//// Generate an image from a text prompt
//var options = new ImageGenerationOptions
//{
//    MediaType = "image/png"
//};

//string prompt = "A tennis court in a jungle";
//var response = await generator.GenerateImagesAsync(prompt, options);


//// Save the image to a file.
//var dataContent = response.Contents.OfType<DataContent>().First();
//string fileName = SaveImage(dataContent, "jungle-tennis.png");
//Console.WriteLine($"Image saved to file: {fileName}");

//static string SaveImage(DataContent content, string fileName)
//{
//    string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
//    var path = Path.Combine(userDirectory, fileName);
//    File.WriteAllBytes(path, content.Data.ToArray());
//    return Path.GetFullPath(path);
//}