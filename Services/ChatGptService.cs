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

    public record StartupInput(string Country, string City, string ProjectName, string Idea);
    public record Problem(string Title, string Description);
    public record ProblemsPayload(List<Problem> Problems);

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
                    $"Generate exactly {count} potential problems a startup might face. " +
                    "Return JSON that matches the provided schema only."),
                new UserChatMessage(
                    $"Country: {input.Country}\n" +
                    $"City: {input.City}\n" +
                    $"Project: {input.ProjectName}\n" +
                    $"Idea: {input.Idea}")
            },
            options);

        var raw = completion.Value.Content[0].Text;
        return JsonSerializer.Deserialize<ProblemsPayload>(raw)!;
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
