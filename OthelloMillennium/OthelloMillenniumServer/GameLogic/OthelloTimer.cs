using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloMillenniumServer.GameLogic
{
    interface ITimer
    {
        void Start();
        void Stop();
        void Reset();
        long GetRemainingTime();
        bool IsRunning();
    }
}
