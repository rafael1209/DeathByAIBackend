namespace DeathByAIBackend.Models
{
    public class StartupInitDto
    {
        public string AuthToken { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public string Idea { get; set; } = string.Empty;
    }
}
