using System;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var sv = new CServer();


            Console.WriteLine("Server started on port {0}", 4040);
            Console.ReadLine();
        }
    }
}