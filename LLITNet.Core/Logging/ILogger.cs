namespace LLITNet.Core.Logging
{
    using System;

    public interface ILogger
    {
        void Log(string msg, params string[] subPaths);
    }

    public interface IGenericLogger : ILogger
    {
        void Log<T>(T item, Action<T> callback = null, params string[] subPaths);
    }
}
