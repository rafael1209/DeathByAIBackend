using System;
using System.Text.Json;
using DeathByAIBackend.Interfaces;
using NJsonSchema;
using OpenAI;
using OpenAI.Chat;
using JsonSchema = NJsonSchema.JsonSchema;

namespace DeathByAIBackend.Services;

public sealed class ChatGptService : IAIService
{
    private readonly OpenAIClient _client;
    private const string Model = "gpt-4o";

    public ChatGptService(IConfiguration cfg)
    {
        _client = new OpenAIClient(cfg.GetValue<string>("ChatGpt:ConnectionString"));
    }

    public async Task<string> SendTextQueryAsync(string query)
    {
        var chat = _client.GetChatClient(Model);
        var result = await chat.CompleteChatAsync(new ChatMessage[]
        {
            new UserChatMessage(query)
        });

        return result.Value.Content[0].Text;
    }

    // ---------- business-problem pipeline ----------

    public record StartupInput(string Location, string ProjectName, string Idea);
    public class Problem
    {
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }

    public class ProblemsPayload
    {
        public List<Problem> problems { get; set; } = new List<Problem>();
    }

    public async Task<ProblemsPayload> GenerateProblemsAsync(
        StartupInput input,
        int count,
        float temperature = 0f)
    {
        var schemaJson = CreateSchema(count);

        var options = new ChatCompletionOptions
        {
            Temperature = temperature,
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "problems",
                jsonSchema: BinaryData.FromString(schemaJson),
                jsonSchemaIsStrict: true)
        };

        var chat = _client.GetChatClient(Model);

        var completion = await chat.CompleteChatAsync(
            new ChatMessage[]
            {
                new SystemChatMessage(
                    $"Generate exactly {count} concrete challenges that any startup might encounter worldwide. " +
                    "Return ONLY the following JSON structure (no extra text):\\n\\n" +
                    "{{\\n  \\\"result\\\": {{\\n    \\\"problems\\\": [ /* exactly {count} objects */ ]\\n  }}\\n}}\\n\\n" +

                    "Each element of \\\"problems\\\" MUST match exactly:\\n" +
                    "{{\\n  \\\"title\\\": \\\"<short title>\\\",\\n  \\\"description\\\": \\\"<detailed explanation>\\\"\\n}}\\n\\n" +

                    "Rules for every \\\"description\\\":\\n" +
                    "• Write 2–4 full sentences.\\n" +
                    "• Insert the user's location string exactly once, at a random position in the text (not always at the start).\\n" +
                    "• Describe 1–2 concrete pain-points; whenever relevant, mention local regulations, cultural or infrastructure specifics for that location.\\n" +
                    "• End with the exact question: \\\"How will you solve it?\\\"\\n\\n" +

                    "Do NOT add, remove, or rename any fields. Do NOT output anything except the JSON object."
                ),
        new UserChatMessage(
                    $"Location: {input.Location}\n" +
                    $"Project: {input.ProjectName}\n" +
                    $"Idea: {input.Idea}")
            },
            options);

        var raw = completion.Value.Content[0].Text;

        var result = JsonSerializer.Deserialize<ProblemsPayload>(raw)!;

        return result;
    }

    // ---------- schema builder ----------

    private static string CreateSchema(int count)
    {
        var problemSchema = new JsonSchema
        {
            Type = JsonObjectType.Object,
            AllowAdditionalProperties = false
        };
        problemSchema.Properties["title"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.String,
            IsRequired = true
        };
        problemSchema.Properties["description"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.String,
            IsRequired = true
        };

        var schema = new JsonSchema
        {
            Type = JsonObjectType.Object,
            AllowAdditionalProperties = false
        };
        schema.Properties["problems"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.Array,
            MinItems = count,
            MaxItems = count,
            Item = problemSchema,
            IsRequired = true
        };

        return schema.ToJson();
    }
}
