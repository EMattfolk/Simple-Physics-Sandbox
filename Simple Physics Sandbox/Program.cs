using System;

namespace Simple_Physics_Sandbox
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new PysicsSandboxMain())
                game.Run();
        }
    }
#endif
}
