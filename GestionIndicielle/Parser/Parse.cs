using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Annotations;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace GestionIndicielle.Parser
{
   
    class Parse
    {
        
        public static void Load(Matrice data,DateTime tDeb, DateTime tFin)
        {
  
            using (parseurDataContext pdc = new parseurDataContext())
            {
                    IQueryable<double> values = from n in pdc.HistoComponents
                    where DateTime.Compare(n.date, tFin ) <0  && DateTime.Compare(n.date,tDeb)>0
                    select  n.value;


                IEnumerator<double> e=new DoubleCollection.Enumerator();
                e = values.GetEnumerator();
                e.MoveNext();
                for (int i = 0; i < data.Row; i++)
                {
                    for (int j = 0; j < data.Col; j++)
                    {
                        data.Mat[i,j] = e.Current;
                        Console.WriteLine(data.Mat[i,j]);
                        e.MoveNext();
                    }
                }

               
            }
        }
    }
}
