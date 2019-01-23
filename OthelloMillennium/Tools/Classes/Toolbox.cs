using System;
using System.Text;
using Tools;

namespace Tools
{
    public static class Toolbox
    {
        public static void LogError(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(DateTime.Now.ToLongDateString());
            sb.Append("] : ");
            sb.Append(ex.Message);
            sb.Append(" - ");
            sb.Append(ex.ToString());
            Console.Error.WriteLine(sb.ToString());
        }

        public static bool Connected(OthelloTCPClient client)
        {
            throw new NotImplementedException("[Connected] : Toolbox");
            //bool part1 = client.Socket.Poll(1000, System.Net.Sockets.SelectMode.SelectRead);
            //bool part2 = client.Socket.Available == 0;
            //return !(part1 & part2);
        }
    }
}
