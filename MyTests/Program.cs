using System;
using static System.Console;
using EventB;

namespace MyTests
{
    class Program
    {
        static void RepList_AddRemove()
        {
            RepositoryEventList rs = new RepositoryEventList();

            WriteLine("Add - " + rs.Add(new Event()));
            WriteLine("rem - " + rs.RemoveEvent(5));
        }

        static void Main(string[] args)
        {
            RepList_AddRemove();

            ReadKey();


        }
    }
}
