using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using System.Text;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string? key = config["GeminiApiKey"];

string? model = "gemini-3.5-flash";


var options = new OpenAIClientOptions
{
    Endpoint = new Uri("https://generativelanguage.googleapis.com/v1beta/openai/")
};

var openAiClient = new OpenAIClient(new ApiKeyCredential(key), options);

IChatClient chatClient = openAiClient
    .GetChatClient(model)
    .AsIChatClient();

List<ChatMessage> chatHistory =
    [
        new ChatMessage(ChatRole.System, """
            You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
            You introduce yourself when first saying hello.
            When helping people out, you always ask them for this information
            to inform the hiking recommendation you provide:

            1. The location where they would like to hike
            2. What hiking intensity they are looking for

            You will then provide three suggestions for nearby hikes that vary in length
            after you get that information. You will also share an interesting fact about
            the local nature on the hikes when making a recommendation. At the end of your
            response, ask if there is anything else you can help with.
        """)
    ];

while (true)
{
    Console.WriteLine("Your prompt:");
    string? userPrompt = Console.ReadLine();

    if (userPrompt is null || userPrompt == "exit")
        break;

    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));
    
    Console.WriteLine("AI Response:");

    StringBuilder sbResponse = new();
    await foreach (ChatResponseUpdate item in chatClient.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);

        sbResponse.Append(item.Text);
    }

    string response = sbResponse.ToString();
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));

    Console.WriteLine();
}