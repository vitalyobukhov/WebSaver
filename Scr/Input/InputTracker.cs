using System;
using System.ComponentModel;
using System.Timers;

namespace Scr.Input
{
    // use input activity tracking wrapper
    partial class InputTracker : IDisposable
    {
        public class InputChangedEventArgs : EventArgs
        {
            public DateTime Initial { get; set; }
            public DateTime Changed { get; set; }
        }


        private bool disposed;

        private readonly object timerLock;
        private readonly Timer timer;

        // two input info instances to compare
        private readonly InputInfo initialInfo;
        private InputInfo actualInfo;


        // state
        public bool Enabled
        {
            get
            {
                lock (timerLock)
                    return timer.Enabled;
            }
            set
            {
                lock (timerLock)
                    timer.Enabled = value;
            }
        }

        // user input polling interval
        public double Interval
        {
            get { return timer.Interval; }
            set
            {
                if (value <= 0 || value > Int32.MaxValue)
                    throw new ArgumentOutOfRangeException("value");

                timer.Interval = value;
            }
        }

        // Changed event related 
        public ISynchronizeInvoke SynchronizingObject
        {
            get { return timer.SynchronizingObject; }
            set { timer.SynchronizingObject = value; }
        }


        // occurs when initialInfo != actualInfo
        public event EventHandler<InputChangedEventArgs> Changed; 


        public InputTracker(EventHandler<InputChangedEventArgs> changed)
        {
            var handler = changed;
            if (handler != null)
                Changed += handler;

            disposed = false;

            timerLock = new object();
            timer = new Timer { AutoReset = true, Enabled = false };
            timer.Elapsed += timer_Elapsed;

            initialInfo = InputInfo.Create();
            actualInfo = (InputInfo)initialInfo.Clone();
        }

        public InputTracker() : this(null)
        { }

        ~InputTracker()
        {
            Dispose(false);
        }


        // user input polling
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (timerLock)
            {
                actualInfo.Update();

                // compare user input info
                if (!initialInfo.Equals(actualInfo))
                {
                    timer.Stop();

                    var handler = Changed;
                    if (handler != null)
                    {
                        // fire changed event
                        handler(this, new InputChangedEventArgs
                            {
                                Initial = initialInfo.DateTime,
                                Changed = actualInfo.DateTime
                            });
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    timer.Stop();
                    timer.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // starts user input polling
        public void Start()
        {
            lock (timerLock)
            {
                initialInfo.Update();
                actualInfo = (InputInfo)initialInfo.Clone();
                timer.Start();
            }
        }

        // stopts user input polling
        public void Stop()
        {
            lock (timerLock)
                timer.Stop();
        }
    }
}
