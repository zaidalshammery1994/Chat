using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace Core.Net
{

    public abstract class Comm
    {
        private IPEndPoint ipEndPoint;
        private bool status;

        public IPEndPoint IpEndPoint
        {
            set
            {
                ipEndPoint = value;
            }
            get
            {
                return ipEndPoint;
            }
        }

        public bool RunStatus
        {
            set
            {
                status = value;
            }
            get
            {
                return status;
            }
        }


        public void SerializeData<T>(Socket socket, Protocol.Message<T> msg)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                NetworkStream strm = new NetworkStream(socket);
                formatter.Serialize(strm, msg);
            }
            catch (SerializationException e)
            {
                new Exceptions.SerializationException(e);
                socket.Close();
            }
            catch (Exception e)
            {
                new Exceptions.UnknowException(e);
                socket.Close();
            }
        }

        public Protocol.Message<T> DeserializeData<T>(Socket socket)
        {
            try
            {
                NetworkStream strm = new NetworkStream(socket);
                IFormatter formatter = new BinaryFormatter();
                Protocol.Message<T> msg = (Protocol.Message<T>)formatter.Deserialize(strm);
                return msg;
            }
            catch (SerializationException e)
            {
                new Exceptions.SerializationException(e);
            }
            catch (Exception e)
            {
                new Exceptions.UnknowException(e);
            }
            socket.Close();
            return null;
        }
    }
}
