using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string? key = config["GeminiApiKey"];

string? model = "gemini-3.5-flash";


// Setup client with Google's OpenAI-compatibility endpoint
var options = new OpenAIClientOptions
{
    Endpoint = new Uri("https://generativelanguage.googleapis.com/v1beta/openai/")
};

var openAiClient = new OpenAIClient(new ApiKeyCredential(key), options);

IChatClient chatClient = openAiClient
    .GetChatClient(model)
    .AsIChatClient();

string[] reviews = [
    "Best purchase ever!",
    "Returned it immediately.",
    "The packaging was damaged but otherwise okay."
];

foreach (var review in reviews)
{
    ChatResponse<Sentiment> response = await chatClient.GetResponseAsync<Sentiment>(
        $"What's the sentiment of this review? '{review}'"
    );

    Console.WriteLine($"Review: {review}");
    Console.WriteLine($"Sentiment: {response.Result}\n");
}

string sampleReview = "The battery life is okay, but it gets a little warm when playing games.";

ChatResponse<SentimentRecord> detailedResponse = await chatClient.GetResponseAsync<SentimentRecord>(
    $"Analyze the sentiment of this review: '{sampleReview}'"
);

Console.WriteLine($"Explanation: {detailedResponse.Result.ResponseText}");
Console.WriteLine($"Sentiment: {detailedResponse.Result.ReviewSentiment}");
public enum Sentiment
{
    Positive,
    Negative,
    Neutral
}
public record SentimentRecord(string ResponseText, Sentiment ReviewSentiment);