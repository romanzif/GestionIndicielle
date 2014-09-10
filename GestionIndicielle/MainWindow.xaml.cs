using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
<<<<<<< HEAD
using GestionIndicielle.ViewModel;
=======
using GestionIndicielle.ViewModels;
using GestionIndicielle.ViewModels;
>>>>>>> c456ee509aa6525020892ecfbddcc947d6ee2a9d

namespace GestionIndicielle
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
<<<<<<< HEAD
            this.DataContext = new MainWindowViewModel();
=======
            DataContext = new MainWindowViewModel();
>>>>>>> c456ee509aa6525020892ecfbddcc947d6ee2a9d
        }
    }
}
