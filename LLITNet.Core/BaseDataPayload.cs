namespace LLITNet.Core
{
    using System;

    public class BaseDataPayload
    {
        public bool? IsOK { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public Exception Error { get; set; }
        public bool HasError => Error != null;
    }
}
