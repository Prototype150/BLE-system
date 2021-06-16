
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Diplom.MyMath
{
    public class PositionCalculator
    {
        public static (double X, double Y) WLS((string id, double X, double Y, List<int> values)[] ps)
        {
            (double X, double Y, double d, double w)[] points = new (double X, double Y, double d, double w)[ps.Length];

            for (int i = 0; i < ps.Length; i++)
            {
                points[i].X = ps[i].X;
                points[i].Y = ps[i].Y;
                points[i].d = Math.Pow(10, (double)(-69 - ps[i].values.Average()) / (10 * 2))*100;
            }

            double totalSum = points.Sum(x => x.d);

            for (int i = 0; i < ps.Length; i++)
            {
                points[i].w = 1 / points[i].d;
            }

            Matrix W = new Matrix(points.Length - 1);
            Matrix A = new Matrix(points.Length - 1, 2);
            Matrix b = new Matrix(points.Length - 1, 1);

            for (int i = 0; i < A.size_vertical; i++)
            {
                A[i, 0] = 2 * (points[i].X - points[points.Length - 1].X);
                A[i, 1] = 2 * (points[i].Y - points[points.Length - 1].Y);
                b[i, 0] = ((points[i].X * points[i].X - points[points.Length - 1].X * points[points.Length - 1].X) + (points[i].Y * points[i].Y - points[points.Length - 1].Y * points[points.Length - 1].Y) + (points[points.Length - 1].d * points[points.Length - 1].d - points[i].d * points[i].d));
                W[i, i] = points[i].w;
            }

            A.TransformTransponate();

            Matrix AT = new Matrix();

            AT.Copy(A);

            A.TransformTransponate();
            W.TransformOpposite();

            Matrix AWA = new Matrix();

            AWA.Copy(AT * W * A);

            AWA.TransformOpposite();

            Matrix result = new Matrix();

            result.Copy(AWA * AT * W * b);

            return (result[0, 0],result[1, 0]);
        }
        public static (double X, double Y) LS((string id, double X, double Y, List<int> values)[] ps)
        {
            (double X, double Y, double d)[] points = new (double X, double Y, double d)[ps.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                points[i].X = ps[i].X;
                points[i].Y = ps[i].Y;
                points[i].d = ps[i].values.Average();
            }

            Matrix A = new Matrix(points.Length - 1, 2);
            Matrix b = new Matrix(points.Length - 1, 1);

            for (int i = 0; i < points.Length; i++)
            {
                points[i].d = Math.Pow(10, (double)(-69 - points[i].d) / (10 * 2))*100;
            }

            for (int i = 0; i < A.size_vertical; i++)
            {
                A[i, 0] = 2 * (points[i].X - points[points.Length - 1].X);
                A[i, 1] = 2 * (points[i].Y - points[points.Length - 1].Y);
                b[i, 0] = ((points[i].X * points[i].X - points[points.Length - 1].X * points[points.Length - 1].X) + (points[i].Y * points[i].Y - points[points.Length - 1].Y * points[points.Length - 1].Y) + (points[points.Length - 1].d * points[points.Length - 1].d - points[i].d * points[i].d));
            }

            Matrix AT = new Matrix();
            Matrix ATO = new Matrix();
            Matrix result = new Matrix();

            A.TransformTransponate();
            AT.Copy(A);
            A.TransformTransponate();
            ATO.Copy(AT * A);
            ATO.TransformOpposite();
            result.Copy(ATO * AT * b);

            return (result[0, 0], result[1, 0]);
        }
    }
}
