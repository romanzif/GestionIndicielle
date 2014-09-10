using System;
using System.Windows;
using GestionIndicielle.Models;
using GestionIndicielle.Parser;

namespace GestionIndicielle.ViewModels
{
    class MainWindowViewModel : Window
    {
        public Matrice D,I;
        
         public MainWindowViewModel()
        {
            DateTime tFin = new DateTime(2006, 1, 17, 0, 0, 0);
            DateTime tDebut= new DateTime(2006,1,2,0,0,0);

            D = new Matrice(DaysIgnoreWeekends(tDebut,tFin), 29);
            I = new Matrice(DaysIgnoreWeekends(tDebut, tFin), 1);
            Parse.LoadPrice(D,tDebut,tFin);
            Parse.LoadIndice(I,tDebut,tFin);
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
    }
}
