namespace LLITNet.Core.Sockets
{
    using System;
    public class SocketClientEventArgs : BaseEventArgs
    {
        public object Data { get; set; }
        public Exception Error { get; set; }
    }
}