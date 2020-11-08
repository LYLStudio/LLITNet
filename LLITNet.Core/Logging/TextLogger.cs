namespace LLITNet.Core.Logging
{
    using System;
    using System.IO;

    using LLITNet.Core.Threading;

    public class TextLogger : ILogger
    {
        protected readonly SequenceOperator _op = null;
        protected readonly SequenceOperator _opGenericType = null;

        protected readonly string _rootFolder = string.Empty;

        public event EventHandler<OperationEventArgs> ErrorOccurred;

        public TextLogger()
        {
            _opGenericType = new SequenceOperator(nameof(_opGenericType));
            _opGenericType.OperationErrorOccurred += OnOperationErrorOccurred;

            _op = new SequenceOperator(nameof(_op));
            _op.OperationErrorOccurred += OnOperationErrorOccurred;
        }

        public TextLogger(string rootFolder) : this()
        {
            _rootFolder = rootFolder;
        }

        protected void OnOperationErrorOccurred(object sender, OperationEventArgs e)
        {
            ErrorOccurred?.Invoke(sender, e);
        }

        public virtual void Log(string msg, params string[] subPaths)
        {
            var item = new WorkItem()
            {
                Parameters = new object[] { msg, subPaths },
                Callback = p0 =>
                {
                    var subPath = string.Empty;
                    if (p0[1] is string[] subs)
                    {
                        subPath = string.Join(@"\", subs);
                    }

                    var path = GetLogPath(_rootFolder, subPath);

                    try
                    {
                        var logMsg = $"{p0[0]}";

                        using (var sw = File.AppendText(path))
                        {
                            var logAppendTime = $"[{DateTime.Now:O}]{logMsg}";
                            sw.WriteLine(logAppendTime);
                            sw.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            };

            _op.Enqueue(item);
        }

        private string GetLogPath(params string[] paths)
        {
            string result;
            try
            {
                var logPath = Path.Combine(paths);
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                var now = DateTime.Now;
                result = Path.Combine(logPath, $"{now:yyyyMMdd-HH}.log");

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
