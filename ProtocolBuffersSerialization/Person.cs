using ProtoBuf;

namespace ProtocolBuffersSerialization
{

    [ProtoContract]
    public class Person
    {
        public Person()
        {
        }

        public Person(string firstName, string lastName, int age, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            PhoneNumber = phoneNumber;
        }

        [ProtoMember(1)]
        public string FirstName { get; set; }
        [ProtoMember(2)]
        public string LastName { get; set; }
        [ProtoMember(3)]
        public int Age { get; set; }
        [ProtoMember(4)]
        public string PhoneNumber { get; set; }
        
    }
}
