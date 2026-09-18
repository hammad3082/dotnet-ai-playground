using Google.GenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DataIngestion;
using Microsoft.Extensions.DataIngestion.Chunkers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.ML.Tokenizers;
using Microsoft.SemanticKernel.Connectors.SqliteVec;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string? key = config["GeminiApiKey"];

string? chatModel = "gemini-3.5-flash-lite";
string? embeddingModel = "gemini-embedding-2";// "text-embedding-001";//gemini-embedding-2

var client = new Client(apiKey: key);
IChatClient chatClient = client.AsIChatClient(chatModel);

IngestionDocumentReader reader = new MarkdownReader();

using ILoggerFactory loggerFactory =
    LoggerFactory.Create(builder => builder.AddSimpleConsole());


// Configure document processor.
EnricherOptions enricherOptions = new(chatClient)
{
    // Enricher failures should not fail the whole ingestion pipeline,
    // as they are best-effort enhancements.
    // This logger factory can create loggers to log such failures.
    LoggerFactory = loggerFactory
};

IngestionDocumentProcessor imageAlternativeTextEnricher =
    new ImageAlternativeTextEnricher(enricherOptions);

IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
    client.AsIEmbeddingGenerator(embeddingModel);

// Configure chunker to split text into semantic chunks.
IngestionChunkerOptions chunkerOptions = new(TiktokenTokenizer.CreateForModel("gpt-4o"))
{
    MaxTokensPerChunk = 50,
    OverlapTokens = 0
};

IngestionChunker<string> chunker =
    new SemanticSimilarityChunker(embeddingGenerator, chunkerOptions);

// Configure chunk processor to generate summaries for each chunk.
IngestionChunkProcessor<string> summaryEnricher = new SummaryEnricher(enricherOptions);


// Configure SQLite Vector Store.
using SqliteVectorStore vectorStore = new(
    "Data Source=vectors.db;Pooling=false",
    new()
    {
        EmbeddingGenerator = embeddingGenerator
    });

// The writer requires the embedding dimension count to be specified.
using VectorStoreWriter<string> writer = new(
    vectorStore,
    dimensionCount: 3072,
    new VectorStoreWriterOptions { CollectionName = "data" });

//// Gemini gemini-embedding-001 outputs 3072 dimensions by default (or can be truncated)
//using VectorStoreWriter<string> writer = new(
//    vectorStore,
//    dimensionCount: 3072,
//    new VectorStoreWriterOptions { CollectionName = "document_chunks" }
//);

// Compose data ingestion pipeline
using IngestionPipeline<string> pipeline =
    new(reader, chunker, writer, loggerFactory: loggerFactory)
    {
        DocumentProcessors = { imageAlternativeTextEnricher },
        ChunkProcessors = { summaryEnricher }
    };

await foreach (IngestionResult result in pipeline.ProcessAsync(
    new DirectoryInfo("./data"),
    searchPattern: "*.md"))
{
    Console.WriteLine($"Completed processing '{result.DocumentId}'. " +
        $"Succeeded: '{result.Succeeded}'.");
}

// Search the vector store collection and display results
VectorStoreCollection<object, Dictionary<string, object?>> collection =
    writer.VectorStoreCollection;

while (true)
{
    Console.Write("Enter your question (or 'exit' to quit): ");
    string? searchValue = Console.ReadLine();
    if (string.IsNullOrEmpty(searchValue) || searchValue == "exit")
    {
        break;
    }

    Console.WriteLine("Searching...\n");
    await foreach (VectorSearchResult<Dictionary<string, object?>> result in
        collection.SearchAsync(searchValue, top: 1))
    {
        Console.WriteLine($"Score: {result.Score}\n\tContent: {result.Record["content"]}");
    }
}