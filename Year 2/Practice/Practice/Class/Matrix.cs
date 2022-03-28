﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Class
{
    /// <summary>
    /// Клласс матрицы
    /// </summary>
    public class Matrix
    {
        private double[,] data;                                     //Значения матрицы 
        private double precalculatedDeterminant = double.NaN;       //Определитель
        private int rows;                                           //Кол-во строк
        private int columns;                                        //Кол-во столбцов
        public int Rows { get => this.rows; }
        public int Columns { get => this.columns; }
        public double[,] Data { get => this.data; }

        //Проверка на квадратность матрицы
        public bool IsSquare { get => this.Rows == this.Columns; }
        //Проверка на трехдиагональность матрицы
        public bool IsTridiagonal { get => CheckTridiagonalMatrix(); }
        //Проверка на симетричность
        public bool IsSymmetry { get => CheckSymmetryMatrix(); }
        //Горизонтальная норма 
        public double NormColum { get => CalculateNormColum(); }
        //Вертикальная норм 
        public double NormRow { get => CalculateNormRow(); }

        public Matrix(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            this.data = new double[rows, columns];
            this.ProcessFunctionOverData((i, j) => this.data[i, j] = 0);
        }

        public Matrix(double[,] initArray)
        {
            this.rows = initArray.GetLength(0);
            this.columns = initArray.GetLength(1);
            this.data = new double[this.rows, this.columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    this.data[i, j] = initArray[i, j];
        }

        public void SwapRows(int x, int y)
        {
            for (int i = 0; i < Columns; i++)
            {
                double temp = this[x, i];
                this[x, i] = this[y, i];
                this[y, i] = temp;
            }
        }
       

        /// <summary>
        /// Функция обработки данных
        /// </summary>
        /// <param name="func">Операция для изменения элементов</param>
        public void ProcessFunctionOverData(Action<int, int> func)
        {
            for (var i = 0; i < this.Rows; i++)
                for (var j = 0; j < this.Columns; j++)
                    func(i, j);
        }

        /// <summary>
        /// Обращение к элементу матрицы
        /// </summary>
        /// <param name="x">Строка</param>
        /// <param name="y">Столбец</param>
        /// <returns></returns>
        public double this[int x, int y]
        {
            get
            {
                return this.data[x, y];
            }
            set
            {
                this.data[x, y] = value;
                this.precalculatedDeterminant = double.NaN;
            }
        }


        /// <summary>
        /// Сложение двух матриц
        /// </summary>
        /// <param name="matrixOne">Первая матрица</param>
        /// <param name="matrixTwo">Вторая матрица</param>
        /// <returns></returns>
        public static Matrix operator +(Matrix matrixOne, Matrix matrixTwo)
        {
            if (matrixOne.Rows != matrixTwo.Rows || matrixOne.Columns != matrixTwo.Columns)
                throw new ArgumentException("Эти матрицы не могут быть сложены");

            var result = new Matrix(matrixOne.Rows, matrixOne.Columns);
            result.ProcessFunctionOverData((i, j) => result[i, j] = matrixOne[i, j] + matrixTwo[i, j]);
            return result;
        }

        /// <summary>
        /// Вычитание двух матриц
        /// </summary>
        /// <param name="matrix1">Первая матрица</param>
        /// <param name="matrix2">Вторая матрица</param>
        /// <returns></returns>
        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            return matrix1 + (matrix2 * -1);
        }

        /// <summary>
        /// Операция умножения матрицы на число
        /// </summary>
        /// <param name="matrix">Матрица</param>
        /// <param name="number">Число</param>
        /// <returns></returns>
        public static Matrix operator *(Matrix matrix, double number)
        {
            var result = new Matrix(matrix.Rows, matrix.Columns);
            result.ProcessFunctionOverData((i, j) => result[i, j] = matrix[i, j] * number);
            return result;
        }

        /// <summary>
        /// Операция умножения матрицы на вектор
        /// </summary>
        /// <param name="matrix">Матрица</param>
        /// <param name="vector">Вектор</param>
        /// <returns></returns>
        public static Vector operator *(Matrix matrix, Vector vector)
        {
            if (matrix.Columns != vector.Count)
                throw new ArgumentException("Эти матрицы не могут быть умножены");

            var result = new Matrix(matrix.Rows, 1);
            result.ProcessFunctionOverData((i, j) =>
            {
                for (var k = 0; k < matrix.Columns; k++)
                {
                    result[i, j] += matrix[i, k] * vector[k];
                }
            });
            return new Vector(result);
        }

        /// <summary>
        /// Операция умножения двух матриц
        /// </summary>
        /// <param name="matrixOne">Первая матрица</param>
        /// <param name="matrixTwo">Вторая матрица</param>
        /// <returns></returns>
        public static Matrix operator *(Matrix matrixOne, Matrix matrixTwo)
        {
            if (matrixOne.Columns != matrixTwo.Rows)
                throw new ArgumentException("Эти матрицы не могут быть умножены");

            var result = new Matrix(matrixOne.Rows, matrixTwo.Columns);
            result.ProcessFunctionOverData((i, j) =>
            {
                for (var k = 0; k < matrixOne.Columns; k++)
                {
                    result[i, j] += matrixOne[i, k] * matrixTwo[k, j];
                }
            });
            return result;
        }

        /// <summary>
        /// Создание единичной матрицы 
        /// </summary>
        /// <param name="size">Размер матрицы</param>
        /// <returns></returns>
        public static Matrix CreateIdentityMatrix(int size)
        {
            var result = new Matrix(size, size);
            for (var i = 0; i < size; i++)
                result[i, i] = 1;

            return result;
        }

        /// <summary>
        /// Транспонирование матрицы
        /// </summary>
        /// <returns></returns>
        public Matrix CreateTransposeMatrix()
        {
            var result = new Matrix(this.Columns, this.Rows);
            result.ProcessFunctionOverData((i, j) => result[i, j] = this[j, i]);
            return result;
        }

        /// <summary>
        /// Определитель
        /// </summary>
        /// <returns></returns>
        public double CalculateDeterminant()
        {
            if (!double.IsNaN(this.precalculatedDeterminant))
                return this.precalculatedDeterminant;

            if (!this.IsSquare)
                throw new InvalidOperationException("Определитель существует только у квадратных матриц");

            if (this.Columns == 1)
                return this[0, 0];
            if (this.Columns == 2)
                return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];

            double result = 0;
            for (var j = 0; j < this.Columns; j++)
                result += (j % 2 == 1 ? 1 : -1) * this[1, j] *
                    this.CreateMatrixWithoutColumn(j).CreateMatrixWithoutRow(1).CalculateDeterminant();

            this.precalculatedDeterminant = result;
            return result;
        }

        /// <summary>
        /// Обратная матрица
        /// </summary>
        /// <returns></returns>
        public Matrix CreateInvertibleMatrix()
        {
            if (this.Rows != this.Columns)
                return null;
            double determinant = CalculateDeterminant();
            if (Math.Abs(determinant) < Constants.DoubleComparisonDelta)
                return null;

            Matrix result = new Matrix(Rows, Rows);
            ProcessFunctionOverData((i, j) =>
            {
                result[i, j] = ((i + j) % 2 == 1 ? -1 : 1) * CalculateMinor(i, j) / determinant;
            });

            result = result.CreateTransposeMatrix();
            return result;
        }
        
        /// <summary>
        /// Подсчет минора
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private double CalculateMinor(int i, int j)
        {
            return CreateMatrixWithoutColumn(j).CreateMatrixWithoutRow(i).CalculateDeterminant();
        }

        /// <summary>
        /// Создать матрицу без строки
        /// </summary>
        /// <param name="row">Cтрока</param>
        /// <returns></returns>
        private Matrix CreateMatrixWithoutRow(int row)
        {
            if (row < 0 || row >= this.Rows)
                throw new ArgumentException("Строка не существует");

            var result = new Matrix(this.Rows - 1, this.Columns);
            result.ProcessFunctionOverData((i, j) => result[i, j] = i < row ? this[i, j] : this[i + 1, j]);
            return result;
        }

        /// <summary>
        /// Создать матрицу без столбца
        /// </summary>
        /// <param name="row">Cтолбец</param>
        /// <returns></returns>
        private Matrix CreateMatrixWithoutColumn(int column)
        {
            if (column < 0 || column >= this.Columns)
                throw new ArgumentException("Столбец не существует");
            var result = new Matrix(this.Rows, this.Columns - 1);
            result.ProcessFunctionOverData((i, j) => result[i, j] = j < column ? this[i, j] : this[i, j + 1]);
            return result;
        }

        /// <summary>
        /// Вывод матрицы
        /// </summary>
        public void Print()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                    Console.Write("{0} ", data[i, j]);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Метод позволяющий заменить нужный столбец
        /// </summary>
        /// <param name="vector">Вектор столбца</param>
        /// <param name="index">Номер столбца</param>
        /// <returns></returns>
        public Matrix InsertColumn(Vector vector, int index)
        {
            if(this.Columns != vector.Count)
                throw new ArgumentException("Расхождение с размерами");

            Matrix rez = new Matrix(this.data);
            for (int i = 0; i < rez.rows; i++)
                rez[i, index] = vector[i];
            return rez;
        }

        /// <summary>
        /// Копирование матрицы
        /// </summary>
        /// <returns></returns>
        public Matrix Copy()
        {
            Matrix temp = new Matrix(this.Rows, this.Columns);
            for (int i = 0; i < this.Rows; i++)
                for (int j = 0; j < this.Columns; j++)
                    temp[i, j] = this[i, j];

            return temp;
        }

        /// <summary>
        /// Проверка на трехдиагональную матрицу
        /// </summary>
        /// <returns></returns>
        private bool CheckTridiagonalMatrix()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (i == j || i == j - 1 || i == j + 1)
                        continue;

                    if (this[i, j] != 0)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Основное условие сходимости итерационного метода
        /// </summary>
        /// <returns></returns>
        private bool BasicConditionIteratAlgo()
        {
            double sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                    if (i != j)
                        sum += Math.Abs(this[i, j]);
                if (Math.Abs(this[i, i]) < sum)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Проверка основного условия итерационного метода
        /// </summary>
        /// <param name="matrix">Проверяемая матрица коэффициентов системы</param>
        /// <param name="vector">Проверяемый вектор свободных членов системы</param>
        /// <returns></returns>
        public bool ChecIteratAlgo(Vector vector)
        {
            int last = 1;
            do
            {
                last = Permutations.NextPermutation(this, vector, last);
                if (this.BasicConditionIteratAlgo())
                    return true;
            } while (last != 1);

            return false;
        }

        /// <summary>
        /// Проверка на симметричность матрицы
        /// </summary>
        /// <returns></returns>
        private bool CheckSymmetryMatrix()
        {
            if (!this.IsSquare)
                return false;
            for (int i = 0; i < Rows; i++)
                for (int j = i; j < Columns; j++)
                    if (this[i, j] != this[j, i])
                        return false;

            return true;
        }

        /// <summary>
        /// Подсчет нормы по строкам
        /// </summary>
        /// <returns></returns>
        private double CalculateNormColum()
        {
            double max = double.MinValue;
            for (int i = 0; i < Rows; i++)
            {
                double temp = 0;
                for (int j = 0; j < Columns; j++)
                    temp += data[i, j];
                max = Math.Max(max, temp);
            }
            return max;
        }

        /// <summary>
        /// Подсчет нормы столбцам
        /// </summary>
        /// <returns></returns>
        private double CalculateNormRow()
        {
            double max = double.MinValue;
            for (int i = 0; i < Rows; i++)
            {
                double temp = 0;
                for (int j = 0; j < Columns; j++)
                    temp += data[j, i];
                max = Math.Max(max, temp);
            }
            return max;
        }

        /// <summary>
        /// Сортировка строк
        /// </summary>
        /// <param name="matrix">Сортируемая матрица</param>
        /// <param name="value">Вектор</param>
        /// <param name="index">Индекс</param>
        public static void SortRows(Matrix matrix, Vector value, int index)
        {
            double maxElement = matrix[index, index];
            int maxElementIndex = index;
            for (int i = index + 1; i < matrix.Columns; i++)
            {
                if (matrix[i, index] > maxElement)
                {
                    maxElement = matrix[i, index];
                    maxElementIndex = i;
                }
            }

            if (maxElementIndex > index)
            {
                double temp;

                temp = value[maxElementIndex];
                value[maxElementIndex] = value[index];
                value[index] = temp;

                for (int i = 0; i < matrix.Rows; i++)
                {
                    temp = matrix[maxElementIndex, i];
                    matrix[maxElementIndex, i] = matrix[index, i];
                    matrix[index, i] = temp;
                }
            }
        }
    }
}
