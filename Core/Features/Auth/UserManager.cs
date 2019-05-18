using System.Collections.Generic;

namespace Core.Features.Auth
{

    public class UserManager
    {
        private List<User> userList;
        private Serialization serialization;


        public List<User> UserList
        {
            set
            {
                userList = value;
            }
            get
            {
                return userList;
            }
        }


        public UserManager(string path)
        {
            serialization = new Serialization();
            Init(path);
        }

        public bool Login(string name, string password)
        {
            foreach (var item in UserList)
            {
                if (item.Name == name)
                    if (item.Password == password)
                        return true;
                continue;
            }
            return false;
        }


        public bool Register(string name, string password)
        {
            foreach (var item in UserList)
            {
                if (item.Name == name)
                    return false;
            }

            UserList.Add(new User(name, password));
            return true;
        }

        public void Init(string path)
        {
            List<User> temp = serialization.Load<User>(path);
            if (temp != null)
                UserList = temp;
            else
                UserList = new List<User>();
        }

        public bool Save(string path)
        {
            if (serialization.Save(UserList, path))
                return true;
            return false;
        }
    }
}
