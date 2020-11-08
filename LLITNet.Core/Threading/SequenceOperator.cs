namespace LLITNet.Core.Threading
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    public class SequenceOperator : IDisposable
    {
        private readonly Thread _thread;
        private readonly AutoResetEvent _resetEvent;
        private readonly ConcurrentQueue<WorkItem> _queue;
        private bool _disposedValue;

        public event EventHandler<OperationEventArgs> OperationErrorOccurred;

        public string Name { get; private set; }

        public uint Sleep { get; set; }

        public SequenceOperator(string name = "", uint sleep = 10)
        {
            Name = name;
            Sleep = sleep;

            _resetEvent = new AutoResetEvent(false);
            _queue = new ConcurrentQueue<WorkItem>();
            _thread = new Thread(Start)
            {
                IsBackground = true,
                Name = Name
            };
            _thread.Start();
        }

        public void Enqueue(WorkItem workItem)
        {
            try
            {
                _queue?.Enqueue(workItem);
                _resetEvent?.Set();
            }
            catch (Exception ex)
            {
                OnOperationErrorOccurred(ex);
            }
        }

        protected virtual void OnOperationErrorOccurred(Exception ex)
        {
            OperationErrorOccurred?.Invoke(this, new OperationEventArgs(ex.Message, ex));
        }

        protected virtual void OnOperationErrorOccurred(Exception ex, WorkItem item)
        {
            OperationErrorOccurred?.Invoke(this, new OperationEventArgs(ex.Message, ex, item));
        }

        protected virtual void Start()
        {
            while (true && !_disposedValue)
            {
                if (!_queue.IsEmpty)
                {
                    if (_queue.TryDequeue(out var workItem))
                    {
                        try
                        {
                            workItem.Callback(workItem.Parameters);
                        }
                        catch (Exception ex)
                        {
                            OnOperationErrorOccurred(ex, workItem);
                        }
                    }
                }
                else
                {
                    _resetEvent.WaitOne();
                }
                _resetEvent.WaitOne((int)Sleep);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _resetEvent?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
