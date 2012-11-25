using System;

namespace ShellAndSettings
{
    internal class Loader
    {
        [STAThread]
        public static void Main()
        {
            new App().Run();
        }
    }
}
