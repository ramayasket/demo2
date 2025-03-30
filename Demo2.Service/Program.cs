namespace Demo2.Service
{
    public class Program
    {
        public static async Task Main()
        {
            WebApplication app = await Setup.ComposeApplication();

            app.Run();
        }
    }
}
