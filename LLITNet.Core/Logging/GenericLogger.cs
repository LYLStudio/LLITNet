namespace LLITNet.Core.Logging
{
    using System;

    public class GenericLogger : TextLogger, IGenericLogger
    {
        public GenericLogger() : base()
        {
        }

        public GenericLogger(string rootFolder) : base(rootFolder)
        {
        }

        public virtual void Log<T>(T item, Action<T> callback = null, params string[] subPaths)
        {
            if (callback is null)
            {
                return;
            }

            _opGenericType.Enqueue(new Threading.WorkItem()
            {
                Parameters = new object[] { item, callback },
                Callback = (p0) =>
                {
                    (p0[1] as Action<T>).Invoke((T)p0[0]);
                }
            });
        }
    }
}
