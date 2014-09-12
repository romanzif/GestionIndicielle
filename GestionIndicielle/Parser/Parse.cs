using System;
using System.Collections.Generic;
using System.Data.Common;
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
        
        public static double[,]  LoadPrice( string [] titres,DateTime tDeb, DateTime tFin)
        {
            using (parseurDataContext pdc = new parseurDataContext())
            {
                    IQueryable<double> values = from n in pdc.HistoComponents
                    where DateTime.Compare(n.date, tFin ) <=0  && DateTime.Compare(n.date,tDeb)>=0 && titres.Contains(n.name)
                    orderby   n.date
                    orderby n.name
                    select n.value;

                IQueryable<DateTime> date = from n in pdc.HistoComponents
                    where DateTime.Compare(n.date, tFin) <= 0 && DateTime.Compare(n.date, tDeb) >= 0 && titres.Contains(n.name)
                    select n.date;

                int dates = date.Count()/titres.Length;
                double[,] data=new double[dates,titres.Length];

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
                return data;
            }
            
        }

        public static double[,] LoadIndice(string [] titres, DateTime tDeb, DateTime tFin)
        {
            using (parseurDataContext pdc = new parseurDataContext())
            {
                IQueryable<double> values = from n in pdc.HistoIndices
                    where DateTime.Compare(n.date, tFin) <= 0 && DateTime.Compare(n.date, tDeb) >= 0
                    orderby n.name  
            select n.value;

                IQueryable<DateTime> date = from n in pdc.HistoComponents
                                            where DateTime.Compare(n.date, tFin) <= 0 && DateTime.Compare(n.date, tDeb) >= 0 && titres.Contains(n.name)
                                            select n.date;

                int dates = date.Count()/titres.Length;
                double [,] data = new double[dates,1];

                IEnumerator<double> e = new DoubleCollection.Enumerator();
                e = values.GetEnumerator();
                e.MoveNext();
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    for (int i = 0; i < data.GetLength(0); i++)
                    {
                        data[i, j] = e.Current;
                        e.MoveNext();
                        //Console.Write(data[i,j]);
                    }
                }
                return data;
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
