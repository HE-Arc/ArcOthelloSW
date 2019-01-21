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
            Console.Error.WriteLine(sb.ToString());
        }

        public static bool Connected(OthelloTCPClient client)
        {
            var s = client.TcpClient.Client;
            bool part1 = s.Poll(1000, System.Net.Sockets.SelectMode.SelectRead);
            bool part2 = s.Available == 0;
            return !(part1 & part2);
        }
    }
}
