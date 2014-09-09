using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GestionIndicielle
{
    class Matrix
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public double[,] Mat { get; set; }

        public Matrix(int row, int col)
        {
            Row = row;
            Col = col;
            Mat=new double[row,col];
        }


    }
}
