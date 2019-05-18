using System;

namespace Core.Protocol
{

    [Serializable]
    public class Message<T>
    {
        private DataType.Head header;
        private T content;

        public DataType.Head Header
        {
            set
            {
                header = value;
            }
            get
            {
                return header;
            }
        }


        public T Content
        {
            set
            {
                content = value;
            }
            get
            {
                return content;
            }
        }


        public Message(DataType.Head head, T content)
        {
            this.Header = head;
            this.Content = content;
        }
    }
}
