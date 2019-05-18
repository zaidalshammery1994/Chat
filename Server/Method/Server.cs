using Core.Protocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Core.Features.Auth;
using System.Net;
using System.IO;
using Server.Properties;
using System.Threading.Tasks;

namespace Server.Method
{

    public class Server : Core.Net.Server
    {
        private UserManager UsrMana;

        public Dictionary<Socket, string> OnlineUserList;


        public Server(IPEndPoint ipEndPoint) : base(ipEndPoint)
        {
            OnlineUserList = new Dictionary<Socket, string>();
            if (!Directory.Exists(Settings.Default.dataPath))
                Directory.CreateDirectory(Settings.Default.dataPath);
            UsrMana = new UserManager(Settings.Default.dataPath + "usrm");
        }

        public bool Save()
        {
            if (UsrMana.Save(Settings.Default.dataPath + "usrm"))
                return true;
            return false;
        }


        public bool Run(int maxBacklog)
        {
            this.MaxBackLog = maxBacklog;
            if (!this.Start())
                return false;
            this.Listener = new Task(Listen);
            this.Listener.Start();
            return true;
        }


        public void StopRun()
        {
            this.Stop();
        }

        private void Listen()
        {
            while (this.RunStatus)
            {
                try
                {
                    Socket clientSocket;
                    clientSocket = this.ServerSocket.Accept();
                    Console.WriteLine("## CONNECT -- " + clientSocket.RemoteEndPoint + " is connected");
                    this.Processer = new Task(() => ProcessData(clientSocket));
                    this.Processer.Start();
                }
                catch (SocketException e)
                {
                    new Core.Exceptions.SocketException(e);
                }
                catch (Exception e)
                {
                    new Core.Exceptions.UnknowException(e);
                }
            }
        }

        private void ProcessData(Socket socket)
        {
            while (this.RunStatus)
            {
                try
                {
                    Message<List<string>> msg = this.DeserializeData<List<string>>(socket);
                    ParseData(socket, msg);
                }
                catch (SocketException e)
                {
                    new Core.Exceptions.SocketException(e);
                    socket.Close();
                }
                catch (Exception e)
                {
                    new Core.Exceptions.UnknowException(e);
                    socket.Close();
                }
            }
        }


        private void MassTextMsg(string msg)
        {
            Message<List<string>> temp =
                new Message<List<string>>(DataType.Head.MSG, new List<string>() { msg });
            foreach (var item in OnlineUserList)
            {
                this.SerializeData<List<string>>(item.Key, temp);
            }
        }


        private string GetUsr(EndPoint EndPoint)
        {
            foreach (var item in OnlineUserList)
            {
                if (item.Key.RemoteEndPoint == EndPoint)
                    return item.Value;
            }
            return null;
        }


        private List<string> GetUserList()
        {
            lock (OnlineUserList)
            {
                List<string> usrList = new List<string>();
                foreach (var item in OnlineUserList)
                {
                    usrList.Add(item.Value);
                }
                return usrList;
            }
        }

        private void ParseData(Socket socket, Message<List<string>> msg)
        {
            switch (msg.Header)
            {

                case DataType.Head.MSG:
                    Console.WriteLine("## LOG -- " + socket.RemoteEndPoint + " send a message");
                    var usrname = GetUsr(socket.RemoteEndPoint);
                    MassTextMsg(usrname + " : " + msg.Content[0].ToString());
                    break;


                case DataType.Head.GUL:
                    Console.WriteLine("## LOG -- " + socket.RemoteEndPoint + " request online user list");
                    Send<List<string>>(socket, new Message<List<string>>(DataType.Head.GUL, GetUserList()));
                    break;


                case DataType.Head.QUIT:
                    Console.WriteLine("## USER -- " + GetUsr(socket.RemoteEndPoint) + " offline");
                    OnlineUserList.Remove(socket);
                    break;

                case DataType.Head.LOGN:
                    Console.WriteLine("## USER -- " + socket.RemoteEndPoint + " trying to login");
                    if (!OnlineUserList.ContainsValue(msg.Content[0]))
                    {
                        if (UsrMana.Login(msg.Content[0], msg.Content[1]))
                        {
                            Send<List<string>>(socket, new Message<List<string>>
                                (DataType.Head.LOGN, new List<string>() { "success" }));
                            OnlineUserList.Add(socket, msg.Content[0]);
                            Console.WriteLine("## USER -- " + msg.Content[0] + " online");
                        }
                        else
                            Send<List<string>>(socket, new Message<List<string>>
                                (DataType.Head.LOGN, new List<string>() { "Account not exist,or check name or password" }));
                    }
                    else
                        Send<List<string>>(socket, new Message<List<string>>
                                    (DataType.Head.LOGN, new List<string>() { "Account already online" }));
                    break;

                case DataType.Head.REGI:
                    Console.WriteLine("## USER -- " + socket.RemoteEndPoint + " trying to register");
                    if (UsrMana.Register(msg.Content[0], msg.Content[1]))
                        Send<List<string>>(socket, new Message<List<string>>
                            (DataType.Head.REGI, new List<string>() { "success" }));
                    else
                        Send<List<string>>(socket, new Message<List<string>>
                            (DataType.Head.REGI, new List<string>() { "Account already exist" }));
                    break;
            }
        }

        private void Send<T>(Socket socket, Message<T> msg)
        {
            this.SerializeData<T>(socket, msg);
        }
    }
}
