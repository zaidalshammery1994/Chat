using Core.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
namespace Client.Method
{
    public class Client : Core.Net.Client
    {
        public List<Message<List<string>>> Session;

        public Client(IPEndPoint ipEndPoint) : base(ipEndPoint)
        {
            Session = new List<Message<List<string>>>();
        }

        public bool Run()
        {
            if (!this.Connect())
                return false;
            this.Processer = new Task(ProcessData);
            this.Processer.Start();
            return true;
        }


        public void StopRun()
        {
            this.Stop();
        }


        public Message<List<string>> GetSession(DataType.Head head)
        {
            Message<List<string>> temp = null;
            lock (Session)
            {
                if (Session.Count > 0)
                {
                    foreach (var item in Session)
                    {
                        if (item.Header == head)
                        {
                            temp = item;
                        }
                    }
                }
            }
            Session.Remove(temp);
            return temp;
        }


        private void ProcessData()
        {
            while (this.RunStatus)
            {
                try
                {
                    Message<List<string>> msg = this.DeserializeData<List<string>>(this.ClientSocket);
                    ParseData(msg);
                }
                catch (SocketException e)
                {
                    new Core.Exceptions.SocketException(e);
                    this.RunStatus = false;
                    this.ClientSocket.Close();
                    MessageBox.Show("Lost connection,Program will exit");
                    Environment.Exit(0);
                }
                catch (Exception e)
                {
                    new Core.Exceptions.UnknowException(e);
                    this.RunStatus = false;
                    this.ClientSocket.Close();
                    MessageBox.Show("Lost connection,Program will exit");
                    Environment.Exit(0);
                }
            }
        }


        private void ParseData(Message<List<string>> msg)
        {
            lock (Session)
            {
                switch (msg.Header)
                {

                    case DataType.Head.MSG:
                        Console.WriteLine(msg.Content[0].ToString());
                        break;

                    case DataType.Head.GUL:
                        Session.Add(msg);
                        break;


                    case DataType.Head.LOGN:
                        Session.Add(msg);
                        break;


                    case DataType.Head.REGI:
                        Session.Add(msg);
                        break;
                }
            }
        }


        public void Send<T>(Message<T> msg)
        {
            this.SerializeData<T>(this.ClientSocket, msg);
        }
    }
}
