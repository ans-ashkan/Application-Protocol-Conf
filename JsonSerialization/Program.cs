using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using fastJSON;
using Newtonsoft.Json;

namespace JsonSerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            //Example using fastjson library because of its performance instead of jsonConvert , serviceStack text is also good alternative
            SerializeDemo();
            DeserializeDemo();
            Console.ReadLine();
        }

        static void SerializeDemo()
        {
            var p = new Person("Ashkan", "Nourzadeh", 20, "09356691528");
            Console.WriteLine("Serialize Demo :\n");
            var jsObj = fastJSON.JSON.ToJSON(p, new JSONParameters() {EnableAnonymousTypes = true});
            File.WriteAllText(@"Person.json",jsObj);
            Console.WriteLine(jsObj);
            Console.WriteLine();
        }

        static void DeserializeDemo()
        {
            var jsonData = File.ReadAllText(@"Person.json");
            var p = fastJSON.JSON.ToObject<Person>(jsonData);
            Console.WriteLine("DeSerialize Demo :\n");
            Console.WriteLine("{0} , {1} , {2} , {3}", p.FirstName, p.LastName, p.Age, p.PhoneNumber);
            Console.WriteLine();
        }
    }
}
