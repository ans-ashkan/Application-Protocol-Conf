using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ProtocolBuffersSerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            SerializeDemo();
            DeserializeDemo();

            Console.ReadLine();
        }

        static void SerializeDemo()
        {
            var p = new Person("Ashkan", "Nourzadeh", 20, "09356691528");


            Console.WriteLine("Serialize Demo :\n");
            using (var file = File.Create("person.bin"))
            {
                Serializer.Serialize(file, p);
            }
            Console.WriteLine("Serialized to person.bin\n");
        }

        static void DeserializeDemo()
        {
            using (var file = File.OpenRead("person.bin"))
            {
                var p = Serializer.Deserialize<Person>(file);
                Console.WriteLine("DeSerialize Demo :\n");
                Console.WriteLine("{0} , {1} , {2} , {3}", p.FirstName, p.LastName, p.Age, p.PhoneNumber);
                
            }
            Console.WriteLine();
        }
    }
}
