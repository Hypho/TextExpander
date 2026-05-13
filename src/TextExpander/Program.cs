namespace TextExpander;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        // TODO: HookEngine integration (build-phase: placeholder)
        Application.Run();
    }
}
