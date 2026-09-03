using Google.GenAI;
using System.Text;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Google.GenAI.Types;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string? key = config["GeminiApiKey"];

string? model = "gemini-3.5-flash-lite";

var client = new Client(apiKey: key);

Console.WriteLine("Uploading sample sales document...");

string sampleJson = """
{
    "description": "This document contains the sale history data for Contoso products.",
    "sales": [
        {
            "month": "January",
            "by_product": {
                "113043": 15,
                "113045": 12,
                "113049": 2
            }
        },
        {
            "month": "February",
            "by_product": {
                "113045": 22
            }
        },
        {
            "month": "March",
            "by_product": {
                "113045": 16,
                "113055": 5
            }
        }
    ]
}
""";

// Write temp file to upload
string tempFilePath = Path.Combine(Path.GetTempPath(), "monthly_sales.json");
await System.IO.File.WriteAllTextAsync(tempFilePath, sampleJson);


// Upload document to Gemini Files API
var uploadedFile = await client.Files.UploadAsync(
    filePath: tempFilePath,
    config: new UploadFileConfig
    { 
        MimeType = "text/plain",// "application /json",
        DisplayName = "Monthly Sales Report 2026"
    }
    //Type: "application/json"
);

System.IO.File.Delete(tempFilePath);

Console.WriteLine($"File uploaded successfully. URI: {uploadedFile.Uri}");

var chatConfig = new GenerateContentConfig
{
    SystemInstruction = new Content
    {
        Parts = new List<Part>
        {
            Part.FromText("You are an assistant that looks up sales data from provided files and helps visualize information. " +
                          "When asked to generate a graph, chart, or trend, use the Code Execution tool to write and execute Python code to plot and render the image.")
        }
    },
    Tools = new List<Tool>
    {
        // Enables Python sandboxed code execution (Code Interpreter equivalent)
        new Tool { CodeExecution = new ToolCodeExecution() }
    }
};

var contents = new List<Content>
{
    new Content
    {
        Role = "user",
        Parts = new List<Part>
        {
            Part.FromUri(uploadedFile.Uri, uploadedFile.MimeType),
            Part.FromText("How well did product 113045 sell in February? Graph its trend over time.")
        }
    }
};

var response = await client.Models.GenerateContentAsync(model, contents, config: chatConfig);

int imageCounter = 1;
foreach (var candidate in response.Candidates)
{
    foreach (var part in candidate.Content.Parts)
    {
        // Text response
        if (!string.IsNullOrEmpty(part.Text))
        {
            Console.WriteLine($"[ASSISTANT]: {part.Text}");
        }

        // Python code executed by Gemini
        if (part.ExecutableCode != null)
        {
            Console.WriteLine("\n--- [Executed Python Code] ---");
            Console.WriteLine(part.ExecutableCode.Code);
            Console.WriteLine("------------------------------\n");
        }

        if (part.CodeExecutionResult != null)
        {
            Console.WriteLine($"[Code Execution Result]: {part.CodeExecutionResult.Output}");
        }

        // Generated chart image output
        if (part.InlineData != null && part.InlineData.MimeType.StartsWith("image/"))
        {
            byte[] imageBytes = part.InlineData.Data.ToArray();
            string imageName = $"sales_chart_{imageCounter++}.png";
            await System.IO.File.WriteAllBytesAsync(imageName, imageBytes);

            Console.WriteLine($"<Saved chart image: {imageName}>");
        }
    } 
}

// 6. Cleanup remote file
await client.Files.DeleteAsync(name: uploadedFile.Name);