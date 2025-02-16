using System;

[Serializable]
public class User
{
    public string name;
    public string surname;
    public int age;

    public User(string name, string surname, int age)
    {
        this.name = name;
        this.surname = surname;
        this.age = age;
    }
}