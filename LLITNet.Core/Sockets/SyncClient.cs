namespace LLITNet.Core.Sockets
{
    using System;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class SyncClient
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread thread;
        private int checkCount = 0;

        public event EventHandler<SocketClientEventArgs> ErrorOccurred;
        public event EventHandler<SocketClientEventArgs> MessageReceived;
        public event EventHandler<BaseEventArgs> DebugMsgGot;

        public string IP { get; private set; }
        public int Port { get; private set; }

        public SyncClient(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

        public void Connect()
        {
            try
            {
                if (client == null)
                {
                    client = new TcpClient(IP, Port);
                }
                else
                {
                    client.Connect(IP, Port);
                }

                stream = client.GetStream();

                if (thread is null || !thread.IsAlive)
                {
                    thread = new Thread(Receive) { IsBackground = true };
                    thread.Start();
                }

                OnDebugMsgGot($"Connected!", DataIdentityFlag.C);
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            try
            {
                stream?.Close();
                client?.Close();
                stream = null;
                client = null;

                OnDebugMsgGot("Disconnected!", DataIdentityFlag.D);
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        public void Send(string message)
        {
            try
            {
                if (stream != null)
                {
                    var data = Encoding.Default.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    OnDebugMsgGot(message, DataIdentityFlag.S);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        public bool CheckConnect()
        {
            var result = false;

            if (client == null)
            {
                return result;
            }

            var blockingState = client.Client.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                client.Client.Blocking = false;
                client.Client.Send(tmp, 0, 0);

                OnDebugMsgGot($"Connected!", DataIdentityFlag.CHK);
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035) && checkCount <= 3)
                {
                    checkCount++;
                    OnDebugMsgGot("Still Connected, but the Send would block", DataIdentityFlag.CHK);
                }
                else
                {
                    OnDebugMsgGot($"Disconnected: error code {e.NativeErrorCode}!", DataIdentityFlag.CHK);
                }
            }
            finally
            {
                if (checkCount > 3)
                {
                    result = false;
                }
                else
                {
                    result = client.Connected;
                }
                client.Client.Blocking = blockingState;

            }
            return result;
        }

        private void Receive()
        {
            byte[] data = new byte[4096];
            AutoResetEvent resetEvent = new AutoResetEvent(false);

            while (true && client.Connected)
            {
                try
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    var message = Encoding.Default.GetString(data, 0, bytes);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        OnDebugMsgGot(message, DataIdentityFlag.R);
                        OnMessageReceived(message);
                    }
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                }

                resetEvent.WaitOne(10);
            }

            OnDebugMsgGot("Receive loop break!", DataIdentityFlag.CHK);

            Disconnect();
        }

        protected void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, new SocketClientEventArgs() { Data = message });
        }

        protected void OnErrorOccurred(Exception ex)
        {
            ErrorOccurred?.Invoke(this, new SocketClientEventArgs() { Error = ex });
        }

        protected void OnDebugMsgGot(string message, DataIdentityFlag flag)
        {
            var msg = $"[{DateTime.Now:O}][{flag}]{message}";
            DebugMsgGot?.Invoke(this, new BaseEventArgs(msg));
        }
    }
}
