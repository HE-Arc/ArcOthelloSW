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
    }
}
