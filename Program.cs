
namespace DeathByAIBackend;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder()
            .Build()
            .Run();
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webHost =>
            {
                webHost.UseStartup<Startup>();
            });
    }
}