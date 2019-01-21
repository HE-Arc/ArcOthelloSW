using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloMillenniumServer.GameLogic
{
    interface OthelloTimer
    {
        void Start();
        void Stop();
        void Reset();
        long GetRemainingTime();
        bool IsRunning();
    }
}
