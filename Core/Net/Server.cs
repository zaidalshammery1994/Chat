using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace Core.Net
{

    public class Server : Comm
    {

        protected Task Listener;

        protected Task Processer;

        public Socket ServerSocket;
        private int maxBackLog;

        public int MaxBackLog
        {
            set
            {
                maxBackLog = value;
            }
            get
            {
                return maxBackLog;
            }
        }

        public Server(IPEndPoint ipEndPoint)
        {
            this.IpEndPoint = ipEndPoint;
            ServerSocket = null;
            this.Listener = null;
            this.Processer = null;
        }


        protected bool Start()
        {
            try
            {
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerSocket.Bind(this.IpEndPoint);
                ServerSocket.Listen(MaxBackLog);
                this.RunStatus = true;
                return true;
            }
            catch (SocketException e)
            {
                new Exceptions.SocketException(e);
            }
            catch (Exception e)
            {
                new Exceptions.UnknowException(e);
            }
            this.RunStatus = false;
            return false;
        }

        protected void Stop()
        {
            this.RunStatus = false;
            ServerSocket.Close();
        }
    }
}
