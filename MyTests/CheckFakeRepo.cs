using System;
using static  System.Console;
using System.Collections.Generic;
using System.Text;
using EventB.Data;

namespace MyTests
{
    public class CheckFakeRepo
    {
        DataProviderList Dat { get; set; }

        public CheckFakeRepo(int nP,int nE,int nV)
        {
            Dat = new DataProviderList();
            Dat.InitRND(nP, nE, nV);

            for (int i = 1; i <= Dat.Persons.Count; i++)
            {
                WriteLine(Dat.GetPersonById(i));
            }
            for (int i = 1; i <= Dat.Events.Count; i++)
            {
                WriteLine(Dat.GetEventById(i));
            }
        }

    }
}
