using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBinarySerialization
{
    class Program
    {
        private static byte[] serializedObject;

        static void Main(string[] args)
        {
            //Jahat e demo subObject ezafe shavad be protobuf

            var person = new Person("Ashkan", "Nourzadeh", 20, "09356691528");

            SerializePerson(person);
            var p = DeserializePerson(serializedObject);
            Console.WriteLine("{0} , {1} , {2} , {3}", p.FirstName, p.LastName, p.Age, p.PhoneNumber);
            Console.ReadLine();
        }

        private static Person DeserializePerson(byte[] bytes)
        {
            var p = new Person();
            using (var ms = new MemoryStream(bytes))
            using (var sr = new BinaryReader(ms))
            {
                p.FirstName = sr.ReadString();
                p.LastName = sr.ReadString();
                p.Age = sr.ReadInt32();
                p.PhoneNumber = sr.ReadString();
                return p;
            }
        }

        private static void SerializePerson(Person person)
        {
            using (var ms = new MemoryStream())
            using (var sw = new BinaryWriter(ms))
            {
                sw.Write(person.FirstName);
                sw.Write(person.LastName);
                sw.Write(person.Age);
                sw.Write(person.PhoneNumber);
                serializedObject = ms.ToArray();
            }
        }
    }
}