using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.UserInteractions
{
    public struct CoordinateSystem
    {
        double maxX;
        double minX;
        double maxY;
        double minY;
        public CoordinateSystem(double maxX = 0, double minX = 0, double maxY = 0,double minY = 0)
        {
            this.maxX = maxX; this.minX = minX;this.maxY = maxY;this.minY = minY;
        }

        public (double, double) Point((double X,double Y) point, (double X, double Y) Vertical, (double X, double Y) Horizontal)
        {
            return (0, 0);
        }
    }
}
