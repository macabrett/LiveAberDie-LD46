namespace Macabre2D.Examples.PhysicsTest {

    using System;
    using Macabre2D.Framework;

    public static class Program {

        [STAThread]
        private static void Main() {
            using (var game = new MacabreGame()) {
                game.Run();
            }
        }
    }
}