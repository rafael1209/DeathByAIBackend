using DeathByAIBackend.Interfaces;
using NJsonSchema;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json;
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
                    "• Insert the user's location string exactly once, at a random position.\\n" +
                    "• Include 1–2 concrete pain-points.\\n" +
                    "• Reference at least one specific local element: a law or regulation (cite its official name/number), " +
                    "a notable recent event (within the last 5 years), or a macro-economic indicator that affects startups in that location.\\n" +
                    "• End with the exact question: \\\"How will you solve it?\\\"\\n\\n" +

                    "Do NOT add, remove, or rename fields. Output the JSON object only."
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




    /* ---------- модели ---------- */

    public record UserSolution(string text);

    public class Evaluation
    {
        public int score { get; set; }      // 0-100
        public int eco { get; set; }      // 0-100
        public string feedback { get; set; } = string.Empty;
        public string future { get; set; } = string.Empty;
    }

    public class EvaluationPayload
    {
        public List<Evaluation> evaluations { get; set; } = new();
        public bool survives { get; set; }
        public string reference { get; set; } = string.Empty;
    }

    /* ---------- публичный API ---------- */

    public async Task<EvaluationPayload> EvaluateSolutionsAsync(
        StartupInput startup,
        ProblemsPayload problems,
        IEnumerable<UserSolution> solutions,
        float temperature = .7f)
    {
        var answers = solutions.ToArray();
        if (answers.Length != problems.problems.Count)
            throw new ArgumentException("answers count mismatch");

        var schema = CreateEvaluationSchema(answers.Length);

        var options = new ChatCompletionOptions
        {
            Temperature = temperature,
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "evaluation",
                jsonSchema: BinaryData.FromString(schema),
                jsonSchemaIsStrict: true)
        };

        const string SysPromptTemplate = """
                                         You are a strict VC panel. Score each proposed solution from 0-100 (higher = better).
                                         Rule-of-thumb:
                                         • 0-39 – fatal • 40-69 – mediocre • 70-89 – strong • 90-100 – excellent.

                                         Also assign **eco points** (0-100) that measure how effectively the solution benefits
                                         the environment (higher = greater positive ecological impact).
                                         Eco points do **not** affect the survive decision.

                                         For every problem/solution return **exactly**:
                                         {{
                                           "score": <int 0-100>,
                                           "eco": <int 0-100>,
                                           "feedback": "<1–2 sentences>",
                                           "future": "<1–2 sentences describing what happens to the startup in the next 6–12 months>"
                                         }}

                                         After scoring decide:
                                           "survives": true if avg(score) ≥ 60 **and** no single score < 30, else false.

                                         Add **reference** – a Google search URL that shows information about a well-known,
                                         successful company from the *same country* **and** the *same industry*:
                                           https://www.google.com/search?q=<Company+Name>

                                         Return **only**:
                                         {{
                                           "evaluations": [ …exactly {0} objects… ],
                                           "survives": <bool>,
                                           "reference": "<Google-search URL>"
                                         }}
                                         """;

        var sysPrompt = string.Format(SysPromptTemplate, answers.Length);

        var userPrompt = BuildUserPrompt(startup, problems, answers);

        var chat = _client.GetChatClient(Model);
        var completion = await chat.CompleteChatAsync(
            new ChatMessage[]
            {
                new SystemChatMessage(sysPrompt),
                new UserChatMessage(userPrompt)
            },
            options);

        var raw = completion.Value.Content[0].Text;
        return JsonSerializer.Deserialize<EvaluationPayload>(raw)!;
    }

    /* ---------- utils ---------- */

    private static string BuildUserPrompt(
        StartupInput startup,
        ProblemsPayload problems,
        IReadOnlyList<UserSolution> answers)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Location: {startup.Location}");
        sb.AppendLine($"Project: {startup.ProjectName}");
        sb.AppendLine($"Idea: {startup.Idea}");
        sb.AppendLine();

        for (var i = 0; i < problems.problems.Count; i++)
        {
            sb.AppendLine($"Problem {i + 1}: {problems.problems[i].title}");
            sb.AppendLine(problems.problems[i].description);
            sb.AppendLine($"User solution: {answers[i].text}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string CreateEvaluationSchema(int count)
    {
        var evalSchema = new JsonSchema
        {
            Type = JsonObjectType.Object,
            AllowAdditionalProperties = false
        };
        evalSchema.Properties["score"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.Integer,
            Minimum = 0,
            Maximum = 100,
            IsRequired = true
        };
        evalSchema.Properties["eco"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.Integer,
            Minimum = 0,
            Maximum = 100,
            IsRequired = true
        };
        evalSchema.Properties["feedback"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.String,
            IsRequired = true
        };
        evalSchema.Properties["future"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.String,
            IsRequired = true
        };

        var root = new JsonSchema
        {
            Type = JsonObjectType.Object,
            AllowAdditionalProperties = false
        };
        root.Properties["evaluations"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.Array,
            MinItems = count,
            MaxItems = count,
            Item = evalSchema,
            IsRequired = true
        };
        root.Properties["survives"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.Boolean,
            IsRequired = true
        };
        root.Properties["reference"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.String, // URL
            IsRequired = true
        };

        return root.ToJson();
    }
}
