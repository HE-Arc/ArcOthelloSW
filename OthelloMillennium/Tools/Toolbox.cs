using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloMillenniumServer
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
            Console.Error.Write(sb.ToString());
        }
    }
}
