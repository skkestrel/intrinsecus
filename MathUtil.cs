﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EhT.Intrinsecus
{
    static class CosineLaw
    {
        /// <summary>
        /// Returns the angle between the lines formed by C->A and C->B
        /// </summary>
        static double cosLaw(Point A, Point B, Point C)
        {
            double angle;

            double lengthCA = Math.Sqrt((A.X - C.X)*(A.X - C.X) + (A.Y - C.Y)*(A.Y - C.Y));
            double lengthCB = Math.Sqrt((B.X - C.X)*(B.X - C.X) + (B.Y - C.Y)*(B.Y - C.Y));
            double lengthAB = Math.Sqrt((B.X - A.X)*(B.X - A.X) + (B.Y - A.Y)*(B.Y - A.Y));

            angle = Math.Acos((lengthCB*lengthCB + lengthCA*lengthCA - lengthAB*lengthAB)/(2*lengthCA*lengthCB));
            angle *= 180 / Math.PI;
            return 0;
        }
    }
}
