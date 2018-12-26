using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace OthelloMillenniumServer.GameLogic
{
    class StoppableTimer
    {
        #region Attributs
        private Stopwatch stopWatch;
        private Timer timer;
        private long initialTime;

        #endregion

        #region Event
        public event EventHandler Timeout;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="availableTime">Initial time in [ms]</param>
        public StoppableTimer(long availableTime)
        {
            initialTime = availableTime;
            stopWatch = new Stopwatch();
            timer = new Timer(initialTime);
            timer.Elapsed += OnTimedEvent;
        }

        /// <summary>
        /// Return remaining time
        /// </summary>
        /// <returns></returns>
        public long GetRemainingTime()
        {
            return Math.Max(initialTime - (long)stopWatch.Elapsed.TotalMilliseconds, 0);
        }

        /// <summary>
        /// Start or resume the timer
        /// </summary>
        public void Start()
        {
            timer.Interval = GetRemainingTime();
            stopWatch.Start();
            timer.Start();
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Stop()
        {
            stopWatch.Stop();
            timer.Stop();
        }

        /// <summary>
        /// Reset the timer
        /// </summary>
        public void Reset()
        {
            timer.Stop();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            timer.Stop();
            stopWatch.Stop();
            EventHandler handler = Timeout;
            handler(this, e);
        }
    }
}
