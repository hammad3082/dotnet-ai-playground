using _04_FunctionCalling;
using Google.GenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string? key = config["GeminiApiKey"];

//string? model = "gemini-3.5-flash";
string? model = "gemini-3.5-flash-lite";

IChatClient innerClient = new Client(apiKey: key)
    .AsIChatClient(model);

// Wrap in ChatClientBuilder to enable tool calling middleware
IChatClient client = new ChatClientBuilder(innerClient)
    .UseFunctionInvocation()
    .Build();

var chatOptions = new ChatOptions
{
    Tools = [
                AIFunctionFactory.Create(SampleTools.GetWeather),
                AIFunctionFactory.Create(SampleTools.GetUserProfile),
                AIFunctionFactory.Create(SampleTools.CalculateLoanPayment),
                AIFunctionFactory.Create(SampleTools.ConvertCurrency),
                AIFunctionFactory.Create(SampleTools.SendEmail)
            ]
};

List<ChatMessage> chatHistory = new()
{
    new ChatMessage(ChatRole.System, "You are an intelligent office assistant. Use your tools whenever appropriate to look up data, convert currency, calculate loans, or send emails.")
};

// Weather conversation relevant to the registered function.
chatHistory.Add(new ChatMessage(ChatRole.User,
    "I live in Montreal and I'm looking for a moderate intensity hike. What's the current weather like?"));

Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");

ChatResponse response = await client.GetResponseAsync(chatHistory, chatOptions);
Console.WriteLine($"Assistant >>> {response.Text}");

Console.WriteLine("Next");

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=================================================");
Console.WriteLine("🤖 AI Assistant with Tool Calling Ready!");
Console.WriteLine("Type 'exit' to quit or 'clear' to reset history.");
Console.WriteLine("=================================================\n");
Console.ResetColor();

// 4. Interactive Loop
while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("\nYou: ");
    Console.ResetColor();

    string? input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

    if (input.Equals("clear", StringComparison.OrdinalIgnoreCase))
    {
        chatHistory.RemoveRange(1, chatHistory.Count - 1); // Keep System Prompt
        Console.WriteLine("🧹 History cleared.");
        continue;
    }

    // Append User Message to History
    chatHistory.Add(new ChatMessage(ChatRole.User, input));

    try
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Thinking & executing tools...");
        Console.ResetColor();

        // Send complete chat history + options to AI
        response = await client.GetResponseAsync(chatHistory, chatOptions);

        // Add Assistant's Response to History to maintain context
        chatHistory.AddMessages(response);

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("\nAI: ");
        Console.ResetColor();
        Console.WriteLine(response.Text);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[Error]: {ex.Message}");
        Console.ResetColor();
    }
}