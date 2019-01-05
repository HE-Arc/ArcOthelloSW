using System;
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

        /// <summary>
        /// Used in the communication between client and server
        /// </summary>
        public static class Protocole
        {
            public readonly static string RegisterSuccessful = "RS";
            public readonly static string OpponentFound = "OF";

            public readonly static string StartOfTheGame = "SG";
            public readonly static string EndOfTheGame = "EG";

            public readonly static string OpponentDisconnected = "OD";

            public readonly static string NextTurn = "NT";

            #region Turn
            /// <summary>
            /// Predate X? and Y?
            /// <para/>Must be followed by ET to end the turn
            /// </summary>
            public readonly static string StartOfTurn = "ST";


            public readonly static string X1 = "X1";
            public readonly static string X2 = "X2";
            public readonly static string X3 = "X3";
            public readonly static string X4 = "X4";
            public readonly static string X5 = "X5";
            public readonly static string X6 = "X6";
            public readonly static string X7 = "X7";
            public readonly static string X8 = "X8";
            public readonly static string X9 = "X9";

            public readonly static string Y1 = "Y1";
            public readonly static string Y2 = "Y2";
            public readonly static string Y3 = "Y3";
            public readonly static string Y4 = "Y4";
            public readonly static string Y5 = "Y5";
            public readonly static string Y6 = "Y6";
            public readonly static string Y7 = "Y7";

            /// <summary>
            /// Follow X? and Y?
            /// <para/>Must be predated by ST to end the turn
            /// </summary>
            public readonly static string EndOfTurn = "ET";
            #endregion
        }
    }
}
