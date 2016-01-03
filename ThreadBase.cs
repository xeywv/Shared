/// <summary>
///	Base class for thread that is to perform an operation on a polled interval. 
///
/// The class should be inherited by a worker class that will override methods:
///    OnStart  - Called when thread is started, to setup the class members.
///    OnProcess- Method that is called at a fixed interval. 
///               If OnProcess is going to take a long time to run, then it may 
///               need to check if the thread has been requested to stop by 
///               calling HasStopBeenRequested.
///    OnStop   - Called when thread is shutdown, to destroy class members.
///
/// In the client that own a derived instance of this class use methods 
///    Start - to start the thread running
///    Stop  - to request the thread to stop running (method will return immediately).
///    Join  - to wait till the thread stop running.
///
/// There is also an event ThreadUnexpectedStopEventHandler that is fired if the 
/// OnProcess method throws and exception causing thread to terminates unexpectedly. 
/// The event is fired asynchronously to prevent possible dead locking issues
/// </summary>
using System;
using System.Threading;

namespace Threading
{
    public class ThreadBase
    {
        #region Constants
        const int FailureDelay = 10000; // If failed then wait for 30secs before trying agian.
        #endregion

        #region Member variables
        private ManualResetEvent abortThread = new ManualResetEvent(false); // Use to signal if a stop is requested on the thread
        protected Thread mainThread;                  // Thread that is run
        private volatile int processInterval = 250; // Poll interval on which to call OnProcess (in ms)        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="threadName">Name of thread instance</param>
        /// <param name="processInterval">Poll interval that OnProcess is called in ms</param>
        public ThreadBase(string threadName, int processInterval)
        {
            this.mainThread = new Thread(this.Run);
            this.ThreadName = threadName;
            this.ProcessInterval = processInterval;
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets poll interval that OnProcess is called in ms.
        /// If the thread is already running the ProcessInterval may not take 
        /// effect until the current poll interval has timed out.
        /// </summary>
        public int ProcessInterval
        {
            get { return processInterval; }
            set { processInterval = value; }
        }

        /// <summary>
        /// Gets if the thread has been requested to stop.
        /// </summary>
        protected bool HasStopBeenRequested
        {
            get { return abortThread.WaitOne(0, false); }
        } 
        
        /// <summary>Thread name (passed in the constructor)</summary>
        protected string ThreadName 
        { 
            get { return (this.mainThread != null) ? this.mainThread.Name : string.Empty; }
            set 
            { 
                if (this.mainThread != null) 
                    this.mainThread.Name = value; 
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Called to start the thread
        /// </summary>
        public void Start()
        {
            abortThread.Reset();
            mainThread.Start();
        }

        /// <summary>
        /// Called to request the thread should stop (the method will end immediately).
        /// Use Join to wait for thread to end.
        /// </summary>
        public void Stop()
        {
            abortThread.Set();
        }

        /// <summary>
        /// Blocks calling thread until the thread terminates.
        /// </summary>
        public void Join()
        {
            mainThread.Join();
        }        
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Method is called when thread starts
        /// Override this method to setup thread members
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Method is called at the specified ProcessInterval.
        /// Override this method to perform thread work.
        /// 
        /// Note if ProcessInterval is set to 100ms, and OnProcess takes 200ms to run
        /// this will mean that OnProcess will effectively be called every 300ms
        /// </summary>
        protected virtual void OnProcess()
        {
        }

        /// <summary>
        /// Method is called when thread stops
        /// Override this method to clean up thread members
        /// </summary>
        protected virtual void OnStop()
        {
        }

        /// <summary>
        /// Method used to log messages.
        /// Writes string to debug output.
        /// </summary>
        /// <param name="msg">message to write to log</param>
        protected virtual void WriteLineToLog(string msg)
        {
            Console.WriteLine(msg);
        }

        protected void WriteLineToLog(string msg, params object[] obj)
        {
            WriteLineToLog(string.Format(msg, obj));
        }

        protected virtual void WriteLineToLog(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This is the main method run by the thread.
        /// The method will continuously call OnProcess at poll interval ProcessInterval,
        /// and won't end until Stop is called or OnProcess throws an exception
        /// Just before the method end it call OnStop.
        /// </summary>
        private void Run()
        {
            OnStart();

            while (!HasStopBeenRequested)
            {
                try
                {
                    // Used to signal if thread has terminated.
                    while (true)
                    {
                        // Wait for specified poll interval, or until thread requested to end 
                        int waitResult = WaitForTimeOut(ProcessInterval);
                        if (waitResult == WaitHandle.WaitTimeout)
                            OnProcess();
                        else if (waitResult == 0)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Wait for specified interval to try again, else if abort thread called then end
                    WaitForTimeOut(FailureDelay);
                    WriteLineToLog(ex);
                }
            }

            // Thread ended so stop
            OnStop();
        }
        #endregion

        #region MyRegion
        /// <summary>
        /// Waits for a speicifc time period befoe returning (or returns if user has aborted).
        /// </summary>
        /// <param name="millisecond">Wait time period</param>
        /// <returns> WaitTimeout once timeout completed or 0 if aborted.</returns>
		private int WaitForTimeOut(int millisecond)
        {
            WaitHandle[] waitHandles = new WaitHandle[] { abortThread };
            int result = WaitHandle.WaitTimeout;

            try
            {
                result = WaitHandle.WaitAny(waitHandles, millisecond, false);
            }
            catch(Exception ex)
            {
                WriteLineToLog(ex);
            }

            return result;
        }
        #endregion    
    }
}
