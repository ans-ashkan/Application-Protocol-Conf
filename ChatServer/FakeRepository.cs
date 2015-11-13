using System.Collections.Generic;

namespace ChatServer
{
    public sealed class FakeRepository
    {
        private static FakeRepository _instance;

        public static FakeRepository Instance
        {
            get { return _instance ?? (_instance = new FakeRepository()); }
        }

        private Dictionary<string,string> _userList = new Dictionary<string, string>();

        private FakeRepository()
        {
            _userList.Add("test","test");
            _userList.Add("user","123");
        }

        public bool CheckUsernamePassword(string username, string password)
        {
            string user;
            return _userList.TryGetValue(username, out user) && user == password;
        }
    }
}