using DeathByAIBackend.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using OpenAI;
using OpenAI.Chat;

namespace DeathByAIBackend.Services
{
    public class ChatGptService : IAIService
    {
        private readonly OpenAIClient _client;

        public ChatGptService(IConfiguration configuration)
        {
            _client = new OpenAIClient(new OpenAIAuthentication(configuration.GetValue<string>("ChatGpt:ConnectionString")));
        }

        public async Task<string> SendTextQueryAsync(string query)

        {
            try
            {
                var request = new ChatRequest(
                    new List<Message>
                    {
                        new Message(Role.User, query)
                    },
                    model: "gpt-4o"
                );

                var result = await _client.ChatEndpoint.GetCompletionAsync(request);
                return result.FirstChoice.Message.Content ?? throw new InvalidOperationException("Response content is null.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error while sending message to GPT: {e}");
                return string.Empty;
            }
        }
    }
}
