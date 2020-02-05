using System;
using static System.Console;
using EventB;

namespace MyTests
{
    class Program
    {
        static void RepList_AddRemove()
        {
            //RepositoryEventList rs = new RepositoryEventList();

            //WriteLine("Add - " + rs.Add(new Event()));
            //WriteLine("rem - " + rs.RemoveEvent(5));
        }

        static void Main(string[] args)
        {
            #region проверка фэйк репозитория
            //var repo = new CheckFakeRepo(3, 10, 0);
            #endregion
            #region проверка тег сплиттера 
            //string test1 = "чтото норм, rfrjtnjrf;dsds\\ds,dsd,/,1235fd";
            //WriteLine(test1);
            //TegSplitter_display t1 = new TegSplitter_display(test1);
            //WriteLine(t1.GetSplitTegs());

            //string test2 = "какоето длинное слово";
            //WriteLine(test2);
            //TegSplitter_display t2 = new TegSplitter_display(test2);
            //WriteLine(t2.GetSplitTegs());

            //string test3 = ",,,,,,,,,";
            //WriteLine(test3);
            //TegSplitter_display t3 = new TegSplitter_display(test3);
            //WriteLine(t3.GetSplitTegs());
            #endregion
            #region начальная выборка выводит 4 подходящих результата
            GetStartEvent_LocalDB_select2injectedEvent O = new GetStartEvent_LocalDB_select2injectedEvent();
            WriteLine("End of generation...");
            WriteLine(O.GetRezult());
            #endregion
            ReadKey();
        }
    }
}
