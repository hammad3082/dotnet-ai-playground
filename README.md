# .NET AI Playground

A hands-on playground exploring .NET AI development based on the official [Microsoft .NET AI Documentation](https://learn.microsoft.com/en-us/dotnet/ai).

This repository follows the quickstart tutorials to build local, vendor-neutral AI pipelines using `Microsoft.Extensions.AI` and `Microsoft.Extensions.VectorData`.

---

## Tech Stack & Services

* **LLM & Embeddings:** Google Gemini API (`gemini-3.5-flash`, `gemini-3.5-flash-lite`, `gemini-embedding-2`)
* **Image Generation:** Hugging Face API (`black-forest-labs/FLUX.1-schnell`)
* **Vector Store:** Local SQLite Vector Store (`SqliteVectorStore`)
* **Framework:** .NET 10 / C#

---

## Projects Overview

* **`01-TextSummarizer`** - Basic text processing and summarization workflows.
* **`02-ChatApp`** - Interactive conversational chat implementation using `IChatClient`.
* **`03-StructuredOutput`** - Parsing and enforcing structured JSON schema outputs from LLMs.
* **`04-FunctionCalling`** - Tool calling capabilities allowing the LLM to execute local C# functions.
* **`05-TextToImageAI`** - Image generation integration using Hugging Face models.
* **`06-AIAssistant`** - Context-aware AI assistant combining chat and tool execution.
* **`07-ProcessDataAI-RAG`** - Local Retrieval-Augmented Generation (RAG) pipeline featuring document ingestion, semantic chunking, vector storage, and similarity search.

---

## Getting Started

1. Initialize and configure your local user secrets:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set GeminiApiKey "your-gemini-api-key"
   dotnet user-secrets set HuggingFaceApiKey "your-huggingface-api-key"

2. Navigate into any project folder and execute the project:
   ```bash
   cd 07-ProcessDataAI-RAG
   dotnet run
