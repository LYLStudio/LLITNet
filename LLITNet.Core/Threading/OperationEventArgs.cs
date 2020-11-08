namespace LLITNet.Core.Threading
{
    using System;

    public class OperationEventArgs : BaseEventArgs
    {
        public OperationPayload Data { get; set; }

        public OperationEventArgs()
        {

        }

        public OperationEventArgs(string message, Exception error = null, WorkItem workItem = null) : this()
        {
            Data = new OperationPayload()
            {
                Message = message,
                Error = error,
                WorkItem = workItem
            };
        }
    }
}
