using System;

namespace Core.Features.Auth
{

    [Serializable]
    public class User
    {
        private string name;
        private string password;


        public string Name
        {
            set
            {
                name = value;
            }
            get
            {
                return name;
            }
        }

        public string Password
        {
            set
            {
                password = value;
            }
            get
            {
                return password;
            }
        }

        public User(string name, string password)
        {
            this.Name = name;
            this.Password = password;
        }
    }
}
