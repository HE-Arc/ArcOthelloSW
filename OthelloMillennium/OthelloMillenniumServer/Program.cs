using System;

namespace OthelloMillenniumServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //TEMP for validation
            for(int i = 0; i < Settings.SIZE_HEIGHT; ++i)
            {
                for (int j = 0; j < Settings.SIZE_WIDTH; ++j)
                {
                    Console.Write(GameState.createStartState().gameboard[i, j]);
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
