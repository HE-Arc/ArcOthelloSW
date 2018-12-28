using System;

namespace OthelloMillenniumServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //TEMP for validation
            var t = GameState.CreateStartState();

            t.ApplyMove(('A', 4), GameState.CellState.BLACK);



            //for (int i = 0; i < Settings.SIZE_HEIGHT; ++i)
            //{
            //    for (int j = 0; j < Settings.SIZE_WIDTH; ++j)
            //    {
            //        Console.Write(GameState.CreateStartState().gameboard[i, j]);
            //    }
            //    Console.WriteLine();
            //}

            Console.ReadKey();
        }
    }
}
