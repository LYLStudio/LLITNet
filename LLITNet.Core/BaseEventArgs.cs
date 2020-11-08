namespace LLITNet.Core
{
    using System;

    public class BaseEventArgs : EventArgs
    {
        public string EventId { get; set; }
        public DateTime Time { get; }
        public string Message { get; set; }

        public BaseEventArgs()
        {
            Time = DateTime.Now;
        }

        public BaseEventArgs(string message) : this()
        {
            Message = message;
        }
    }
}
