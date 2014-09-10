using System;
using System.Windows;
using GestionIndicielle.Models;
using GestionIndicielle.Parser;

namespace GestionIndicielle.ViewModels
{
    class MainWindowViewModel : Window
    {
        private Matrice D;
        
         public MainWindowViewModel()
        {
            DateTime tFin = new DateTime(2006, 1, 10, 0, 0, 0);
            DateTime tDebut= new DateTime(2006,1,2,0,0,0);

            D = new Matrice(7, 29);
             Parse.Load(D,tDebut,tFin);
        }
    }
}
