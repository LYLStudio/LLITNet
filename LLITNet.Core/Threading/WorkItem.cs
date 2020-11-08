namespace LLITNet.Core.Threading
{
    using System;

    public class WorkItem
    {
        public object[] Parameters { get; set; }
        public Action<object[]> Callback { get; set; }

        public ItemInfo Info { get; set; }

        public class ItemInfo
        {
            public string BatchId { get; set; }
            public string UserId { get; set; }
            public string ClientIp { get; set; }
            public string FuncName { get; set; }
            public string CalledSource { get; set; }
        }
    }
}
