using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace XmlSerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            SerializeDemo();
            DeserializeDemo();

            Console.ReadLine();
        }

        static string SerializeToXml<T>(T t)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, t);
                return stringWriter.ToString();
            }
        }
        static T DeserializeXml<T>(string xmlString)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var stringReader = new StringReader(xmlString))
            {
                var t = (T)serializer.Deserialize(stringReader);
                return t;
            }
        }

        static void SerializeDemo()
        {
            var p = new Person("Ashkan", "Nourzadeh", 20, "09356691528");
            Console.WriteLine("Serialize Demo :\n");
            Console.WriteLine(SerializeToXml(p));
            Console.WriteLine();
        }

        static void DeserializeDemo()
        {
            var xmlData = File.ReadAllText(@"Person.xml");
            var p = DeserializeXml<Person>(xmlData);
            Console.WriteLine("DeSerialize Demo :\n");
            Console.WriteLine("{0} , {1} , {2} , {3}", p.FirstName, p.LastName, p.Age, p.PhoneNumber);
            Console.WriteLine();
        }
    }
}
