using EndoAshu.StarSavior.Core;

namespace Program
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Settings.Load();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}