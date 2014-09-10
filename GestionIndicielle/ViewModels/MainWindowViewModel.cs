using System;
using System.Security.RightsManagement;
using System.Windows;
using GestionIndicielle.Parser;

namespace GestionIndicielle.ViewModel
{
    class MainWindowViewModel : Window
    {
        private Matrice D;
        
         public MainWindowViewModel()
        {
            DateTime tFin = new DateTime(2006, 1, 10, 0, 0, 0);
            DateTime tDebut= new DateTime(2006,1,2,0,0,0);

            D = new Matrice(6, 29);
             Parse.Load(D,tDebut,tFin);
        }
    }
}
