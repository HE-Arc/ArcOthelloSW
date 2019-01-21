using System;
using System.Diagnostics;
using System.Timers;

namespace OthelloMillenniumServer.GameLogic
{
    class StoppableCounter : ITimer
    {
        #region Attributs
        private Stopwatch stopWatch;
        private long initialTime;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="availableTime">Initial time in [ms]</param>
        public StoppableCounter(long availableTime)
        {
            initialTime = availableTime;
            stopWatch = new Stopwatch();
        }

        /// <summary>
        /// Return remaining time
        /// </summary>
        /// <returns></returns>
        public long GetRemainingTime()
        {
            return (long)stopWatch.Elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// Start or resume the timer
        /// </summary>
        public void Start()
        {
            stopWatch.Start();
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Stop()
        {
            stopWatch.Stop();
        }

        /// <summary>
        /// Reset the timer
        /// </summary>
        public void Reset()
        {
            stopWatch.Reset();
        }

        /// <summary>
        /// Return true if running
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return stopWatch.IsRunning;
        }
    }
}
