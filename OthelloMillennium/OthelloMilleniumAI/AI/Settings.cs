using System;
using System.Collections.Generic;

namespace IAOthelloMillenium
{
    public class Settings
    {
        public static readonly int EMPTY = -1;
        public static readonly int WHITE = 0;
        public static readonly int BLACK = 1;

        public static readonly int SIZE_WIDTH = 9;
        public static readonly int SIZE_HEIGHT = 7;

        public static List<Tuple<int, int>> CORNERS = new List<Tuple<int, int>>()
            {
                new Tuple<int,int>(0,0),
                new Tuple<int,int>(SIZE_WIDTH-1,0),
                new Tuple<int,int>(0,SIZE_HEIGHT-1),
                new Tuple<int,int>(SIZE_WIDTH-1,SIZE_HEIGHT-1),
            };

        public static readonly int DEPTH_SEARCH = 5;
    }
}
