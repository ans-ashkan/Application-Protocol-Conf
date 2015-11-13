using System;

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

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string PhoneNumber { get; set; }
}
