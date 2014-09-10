using System;
using System.Windows;
using GestionIndicielle.Models;
using GestionIndicielle.Parser;

namespace GestionIndicielle.ViewModels
{
    public class MainWindowViewModel : Window
    {
        public Matrice D,I;
        public DateTime Tfin = new DateTime(2006, 1, 17, 0, 0, 0);
        public DateTime Tdebut = new DateTime(2006, 1, 2, 0, 0, 0);
        //public DateTime Tfin { get; private set; }
        //public DateTime Tdebut { get; private set; }
         public MainWindowViewModel()
        {


            D = new Matrice(DaysIgnoreWeekends(Tdebut,Tfin), 29);
            I = new Matrice(DaysIgnoreWeekends(Tdebut, Tfin), 1);
            Parse.LoadPrice(D,Tdebut,Tfin);
            Parse.LoadIndice(I,Tdebut,Tfin);
        }

        private int DaysIgnoreWeekends(DateTime tDebut, DateTime tFin)
        {
           TimeSpan days = tFin.Subtract(tDebut);
           int count = 0;
           for (int a = 0; a < days.Days + 1; a++)
           {
               if (tDebut.DayOfWeek != DayOfWeek.Saturday && tDebut.DayOfWeek != DayOfWeek.Sunday)
                   count++;
               tDebut = tDebut.AddDays(1.0);
           }
           Console.Write(count);
           return count;
        }

        public void Begin(DateTime d)
        {
            Tdebut = d;
        }

        public void End(DateTime d)
        {
            Tfin = d;
        }
      
    }
}
