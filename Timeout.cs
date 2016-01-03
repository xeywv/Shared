using System;

namespace Shared
{
    /// <summary>Provides an interface to a count down timer</summary>
    public interface ITimeout
    {
        /// <summary>Get or sets the interval in ms that the timeout is set to</summary>
        ulong Interval { get; set; }

        /// <summary>Start time count down timer</summary>
        void Start();

        /// <summary>Reset the count down time (restarts the count)</summary>
        void Reset();

        /// <summary>Gets if the count down timer has reached 0</summary>
        bool HasTimedOut { get; }
    }

    /// <summary>Provides a count down timer class</summary>
    public class Timeout : ITimeout
    {
        #region Member variables
        /// <summary>Start time for timer</summary>
        private DateTime startTime = DateTime.Now;        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval">Count down timer period in ms</param>
        public Timeout(ulong interval)
        {
            this.Interval = interval;
        }        
        #endregion

        #region ITimeout Members
        /// <summary>Get or sets the interval in ms that the timeout is set to</summary>
        public ulong Interval { get; set; }

        /// <summary>Start time count down timer</summary>
        public void Start()
        {
            startTime = DateTime.Now;
        }

        /// <summary>Reset the count down time (restarts the count)</summary>
        public void Reset()
        {
            startTime = DateTime.Now;
        }

        /// <summary>Gets if the count down timer has reached 0</summary>
        public bool HasTimedOut
        {
            get { return ((ulong)(DateTime.Now - startTime).TotalMilliseconds >= Interval); }
        }
        #endregion
    }


    /// <summary>Provides a higher performance count down timer class</summary>
    internal class TimeoutHighPerformance : ITimeout
    {
        #region Member variables
        /// <summary>Stopwatch timer</summary>
        private System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval">Count down timer period in ms</param>
        public TimeoutHighPerformance(ulong interval)
        {
            this.Interval = interval;
        }
        #endregion

        #region ITimeout Members
        /// <summary>Get or sets the interval in ms that the timeout is set to</summary>
        public ulong Interval { get; set; }

        /// <summary>Start time count down timer</summary>
        public void Start()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        /// <summary>Reset the count down time (restarts the count)</summary>
        public void Reset()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        /// <summary>Gets if the count down timer has reached 0</summary>
        public bool HasTimedOut
        {
            get { return ((ulong)stopwatch.ElapsedMilliseconds >= Interval); }
        }
        #endregion
    }
}
