using System;

namespace UniGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new UniGame())
                game.Run();
        }
    }
}
