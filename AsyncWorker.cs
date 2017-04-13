using System.Threading;
using System;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;

namespace TestAsyncWorker {

    /// <summary>
    /// AsyncWorker is a helper class for running asynchronous tasks. 
    /// </summary>
    public class AsyncWorker {
        private bool m_cancelationPending;
        private static Object s_countProtector = new Object();
        private int m_count;
        private int m_maxCount;
        AsyncCallback workerCallback;

        DoWorkEventHandler eventHandler;
        /// <summary>
        /// Occurs when [do work].
        /// </summary>
        public event DoWorkEventHandler DoWork;
        /// <summary>
        /// Occurs when [run worker completed].
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;
        /// <summary>
        /// Occurs when [progress changed].
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncWorker"/> class.
        /// </summary>
        public AsyncWorker(int maximumCount) {
            this.m_maxCount = maximumCount;
            this.workerCallback = new AsyncCallback(this.OnRunWorkerCompleted);
            this.eventHandler = new DoWorkEventHandler(this.OnDoWork);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is busy.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy {
            get {
                lock (s_countProtector) {
                    if (this.m_count >= this.m_maxCount) {
                        return true;
                    }
                    return false;
                    //return m_isBusy;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [cancellation pending].
        /// </summary>
        /// <value><c>true</c> if [cancellation pending]; otherwise, <c>false</c>.</value>
        public bool CancellationPending {
            get {
                return this.m_cancelationPending;
            }
        }

        /// <summary>
        /// Runs the worker async.
        /// </summary>
        /// <param name="abortIfBusy">if set to <c>true</c> [abort if busy].</param>
        public bool RunWorkerAsync(bool abortIfBusy) {
            return this.RunWorkerAsync(abortIfBusy, null);
        }


        /// <summary>
        /// Runs the worker async.
        /// </summary>
        /// <param name="abortIfBusy">if set to <c>true</c> [abort if busy].</param>
        /// <param name="argument">The argument.</param>
        public bool RunWorkerAsync(bool abortIfBusy, object argument) {
            if (abortIfBusy && this.IsBusy) {
                return false;
            }
            m_count++;

            eventHandler.BeginInvoke(this, new DoWorkEventArgs(argument), workerCallback, eventHandler);
            return true;
        }

        /// <summary>
        /// Cancels the async.
        /// </summary>
        public void CancelAsync() {
            this.m_cancelationPending = true;
        }

        /// <summary>
        /// Reports the progress.
        /// </summary>
        /// <param name="percentProgress">The percent progress.</param>
        public void ReportProgress(int percentProgress) {
            this.OnProgressChanged(new ProgressChangedEventArgs(percentProgress, null));
        }
        /// <summary>
        /// Reports the progress.
        /// </summary>
        /// <param name="percentProgress">The percent progress.</param>
        /// <param name="userState">State of the user.</param>
        public void ReportProgress(int percentProgress, object userState) {
            this.OnProgressChanged(new ProgressChangedEventArgs(percentProgress, userState));
        }

        /// <summary>
        /// Called when [do work].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDoWork(object sender, DoWorkEventArgs e) {
            if (e.Cancel) {
                return;
            }
            Console.WriteLine("Async started " + DateTime.UtcNow.ToString());
            if (this.DoWork != null) {
                this.DoWork(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ProgressChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnProgressChanged(ProgressChangedEventArgs e) {
            if (this.ProgressChanged != null) {
                this.ProgressChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [run worker completed].
        /// </summary>
        /// <param name="ar">The ar.</param>
        protected virtual void OnRunWorkerCompleted(IAsyncResult ar) {
            DoWorkEventHandler doWorkDelegate = (DoWorkEventHandler)((AsyncResult)ar).AsyncDelegate;
            if (this.RunWorkerCompleted != null) {
                this.RunWorkerCompleted(this, new RunWorkerCompletedEventArgs(ar, null, this.m_cancelationPending));
            }

            Console.WriteLine("Async ended " + DateTime.UtcNow.ToString());
            m_count--;
        }
    }

}
