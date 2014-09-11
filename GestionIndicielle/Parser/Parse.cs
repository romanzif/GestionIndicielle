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
using GestionIndicielle.Models;


namespace GestionIndicielle.Parser
{
   
    class Parse
    {
        
        public static void LoadPrice(double[,] data,DateTime tDeb, DateTime tFin)
        {
            using (parseurDataContext pdc = new parseurDataContext())
            {
                    IQueryable<double> values = from n in pdc.HistoComponents
                    where DateTime.Compare(n.date, tFin ) <=0  && DateTime.Compare(n.date,tDeb)>=0
                    select  n.value;

                IEnumerator<double> e=new DoubleCollection.Enumerator();
                e = values.GetEnumerator();
                e.MoveNext();
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    for (int i = 0; i < data.GetLength(0); i++)
                    {
                        data[i,j] = e.Current;
                        e.MoveNext();   
                    }
                }
            }
        }

        public static void LoadIndice(double[,] data, DateTime tDeb, DateTime tFin)
        {
            using (parseurDataContext pdc = new parseurDataContext())
            {
                IQueryable<double> values = from n in pdc.HistoIndices
                                            where DateTime.Compare(n.date, tFin) <= 0 && DateTime.Compare(n.date, tDeb) >= 0
                                            select n.value;

                IEnumerator<double> e = new DoubleCollection.Enumerator();
                e = values.GetEnumerator();
                e.MoveNext();
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    for (int i = 0; i < data.GetLength(0); i++)
                    {
                        data[i, j] = e.Current;
                        e.MoveNext();
                    }
                }
            }
            
        }

        public void Print(int Row,int Col,double[,] Mat)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    Console.Write(Mat[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

    }
}
