using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using Google.GenAI;
using System.ClientModel;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
string? key = config["GeminiApiKey"];

string? model = "gemini-3.5-flash";// config["ModelName"];


IChatClient client = new OpenAIClient(
    new ApiKeyCredential(key),
    new OpenAIClientOptions { Endpoint = new Uri("https://generativelanguage.googleapis.com/v1beta/openai/") }
)
.GetChatClient(model)
.AsIChatClient();

//---
//var options = new OpenAIClientOptions
//{
//    Endpoint = new Uri("https://generativelanguage.googleapis.com/v1beta/openai/")
//};

//// Pass your Gemini key and options
//var openAiClient = new OpenAIClient(new ApiKeyCredential(key), options);

//// Wrap as IChatClient specifying a Gemini model
//IChatClient client = openAiClient
//    .GetChatClient(model)
//    .AsIChatClient();



//----- gemini lib
//IChatClient client = new Client(apiKey: key)
//    .AsIChatClient(model);



//------ direct open AI model
//IChatClient client =
//    new OpenAIClient(key).GetChatClient(model).AsIChatClient();

string text = File.ReadAllText("benefits.md");
string prompt = $"""
    Summarize the the following text in 20 words or less:
    {text}
    """;


// Submit the prompt and print out the response.
ChatResponse response = await client.GetResponseAsync(
    prompt,
    new ChatOptions { MaxOutputTokens = 400 });
//var response = await client.GetResponseAsync("Hello!");
Console.WriteLine(response);