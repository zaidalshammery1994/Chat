using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Core.Net
{

    public class Client : Comm
    {

        protected Task Processer;

        protected Socket ClientSocket;

        public Client(IPEndPoint ipEndPoint)
        {
            this.IpEndPoint = ipEndPoint;
            ClientSocket = null;
            this.Processer = null;
        }

        protected bool Connect()
        {
            try
            {
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ClientSocket.Connect(this.IpEndPoint);
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
            this.ClientSocket.Close();
        }

    }
}
