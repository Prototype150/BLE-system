using System;
namespace Diplom.MyMath
{
    internal class MatrixException : Exception
    {
        internal MatrixException(string message) : base(message) { }
    }
    public class Matrix
    {
        double[,] elements;
        public int size_vertical { private set; get; }
        public int size_horizontal { private set; get; }
        public Matrix() : this(0) { }
        public Matrix(int size) : this(size, size) { }
        public Matrix(int size_vertical, int size_horizontal)
        {
            this.size_vertical = size_vertical;
            this.size_horizontal = size_horizontal;
            elements = new double[size_vertical, size_horizontal];
        }

        public double this[int i, int j]
        {
            get { return elements[i, j]; }
            set { elements[i, j] = value; }
        }

        public void Print()
        {
            for (int i = 0; i < size_vertical; i++)
            {
                for (int j = 0; j < size_horizontal; j++)
                    Console.Write(elements[i, j] + " ");
                Console.WriteLine();
            }
        }

        public void Copy(Matrix A)
        {
            size_horizontal = A.size_horizontal;
            size_vertical = A.size_vertical;
            elements = new double[size_vertical, size_horizontal];
            for (int i = 0; i < size_vertical; i++)
                for (int j = 0; j < size_horizontal; j++)
                    elements[i, j] = A[i, j];
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix result = new Matrix();
            result.Copy(a);
            for (int i = 0; i < a.size_vertical; i++)
                for (int j = 0; j < a.size_horizontal; j++)
                    result[i, j] += b[i, j];
            return result;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            Matrix result = new Matrix();
            result.Copy(a);
            for (int i = 0; i < a.size_vertical; i++)
                for (int j = 0; j < a.size_horizontal; j++)
                    result[i, j] -= b[i, j];
            return result;
        }

        public static Matrix operator *(Matrix matrix, double b)
        {
            for (int i = 0; i < matrix.size_vertical; i++)
                for (int j = 0; j < matrix.size_horizontal; j++)
                    matrix[i, j] *= b;
            return matrix;
        }

        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.size_horizontal != B.size_vertical)
            {
                throw new MatrixException("Умножение не возможно! Количество столбцов первой матрицы не равно количеству строк второй матрицы.");
            }
            var result = new Matrix(A.size_vertical, B.size_horizontal);
            for (var i = 0; i < A.size_vertical; i++)
            {
                for (var j = 0; j < B.size_horizontal; j++)
                {
                    result[i, j] = 0;

                    for (var k = 0; k < A.size_horizontal; k++)
                    {
                        result[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            return result;
        }

        public static Matrix operator /(Matrix matrix, double b)
        {
            for (int i = 0; i < matrix.size_vertical; i++)
                for (int j = 0; j < matrix.size_horizontal; j++)
                    matrix[i, j] /= b;
            return matrix;
        }

        public void ChangeRemoveLine(int index)
        {
            int p = 0;
            double[,] newEl = new double[--size_vertical, size_horizontal];
            for (int i = 0; i < size_vertical + 1; i++)
            {
                if (i != index)
                {
                    for (int j = 0; j < size_horizontal; j++)
                        newEl[p, j] = elements[i, j];
                    p++;
                }
                else
                    continue;
            }
            elements = newEl;
        }


        public void ChangeRemoveColumn(int index)
        {
            int p = 0;
            double[,] newEl = new double[size_vertical, --size_horizontal];
            for (int i = 0; i < size_horizontal + 1; i++)
            {
                if (i != index)
                {
                    for (int j = 0; j < size_vertical; j++)
                        newEl[j, p] = elements[j, i];
                    p++;
                }
                else
                    continue;
            }
            elements = newEl;
        }


        public void TransformMinors()
        {
            if (size_horizontal != size_vertical)
                throw new MatrixException("Matrix is not squared!");
            Matrix help = new Matrix();
            Matrix result = new Matrix(size_horizontal);
            help.Copy(this);
            for (int i = 0; i < size_horizontal; i++)
            {
                for (int j = 0; j < size_vertical; j++)
                {
                    ChangeRemoveColumn(i);
                    ChangeRemoveLine(j);
                    result[i, j] = FindDeterminant();
                    this.Copy(help);
                }
            }
            this.Copy(result);
        }

        public void TransformAlgebraicAdditions()
        {
            int one = 1;
            for (int i = 0; i < size_vertical; i++)
            {
                for (int j = 0; j < size_horizontal; j++)
                {
                    elements[i, j] *= one;
                    one = -one;
                }
                if (size_horizontal % 2 == 0)
                    one = -one;
            }
        }

        public void TransformTransponate()
        {
            Matrix result = new Matrix(size_horizontal, size_vertical);

            for (int i = 0; i < size_horizontal; i++)
                for (int j = 0; j < size_vertical; j++)
                    result[i, j] = elements[j, i];

            this.Copy(result);
        }

        public void TransformOpposite()
        {
            if (size_horizontal != size_vertical)
                throw new MatrixException("Matrix is not squared!");
            double temp = FindDeterminant();
            TransformMinors();
            TransformAlgebraicAdditions();
            TransformTransponate();
            for (int i = 0; i < size_vertical; i++)
                for (int j = 0; j < size_horizontal; j++)
                    elements[i, j] /= temp;
        }

        public double FindDeterminant()
        {
            if (size_horizontal != size_vertical)
                throw new MatrixException("Matrix is not squared!");
            if (size_horizontal == 1)
                return elements[0, 0];
            else
            {
                Matrix remember = new Matrix();
                remember.Copy(this);
                double result = 0;
                int one = 1;
                Matrix help = new Matrix();
                double[] line = new double[size_horizontal];
                for (int i = 0; i < size_horizontal; i++)
                    line[i] = elements[0, i];
                ChangeRemoveLine(0);
                help.Copy(this);
                for (int i = 0; i < size_horizontal; i++)
                {
                    ChangeRemoveColumn(i);
                    if (line[i] != 0)
                        result += one * line[i] * FindDeterminant();
                    this.Copy(help);
                    one = -one;
                }
                this.Copy(remember);
                return result;
            }
        }

        public double FindNorm()
        {
            double res = 0;
            double sum;
            for (int i = 0; i < size_vertical; i++)
            {
                sum = 0;
                for (int j = 0; j < size_horizontal; j++)
                    sum += Math.Abs(elements[i, j]);
                if (sum > res)
                    res = sum;
            }
            return res;
        }
    }
}