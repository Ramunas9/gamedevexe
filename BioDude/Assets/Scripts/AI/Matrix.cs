using System;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable MemberCanBeMadeStatic.Local

public class Matrix
{
    private int rows;
    private int cols;
    private float[,] matrix;

    public Matrix(int r, int c)
    {
        rows = r;
        cols = c;
        matrix = new float[rows, cols];
    }

    public void randomize()
    {
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            matrix[i, j] = Random.Range(-1f, 1f);
    }

    public Matrix dot(Matrix n)
    {
        Matrix result = new Matrix(rows, n.cols);

        if (cols == n.rows)
            //for each spot in the new matrix
            for (int i = 0; i < rows; i++)
            for (int j = 0; j < n.cols; j++)
            {
                float sum = 0;
                for (int k = 0; k < cols; k++)
                    sum += matrix[i, k] * n.matrix[k, j];

                result.matrix[i, j] = sum;
            }

        return result;
    }

    //Mutation function for genetic algorithm 
    public void mutate(float mutationRate)
    {
        //for each element in the matrix
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < mutationRate)
                //if chosen to be mutated
                matrix[i, j] += Random.Range(-1f, 1f);
        }
    }

    //Creates a single column array from the parameter array
    public static Matrix singleColumnMatrixFromArray(float[] arr)
    {
        Matrix n = new Matrix(arr.Length, 1);
        for (int i = 0; i < arr.Length; i++)
        {
            n.matrix[i, 0] = arr[i];
        }

        return n;
    }

    //for ix1 matrixes adds one to the bottom
    public Matrix addBias()
    {
        Matrix n = new Matrix(rows + 1, 1);
        for (int i = 0; i < rows; i++)
        {
            n.matrix[i, 0] = matrix[i, 0];
        }

        n.matrix[rows, 0] = 1;
        return n;
    }

    //applies the activation function(sigmoid) to each element of the matrix
    public Matrix activate()
    {
        Matrix n = new Matrix(rows, cols);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                n.matrix[i, j] = sigmoid(matrix[i, j]);
            }
        }

        return n;
    }

    public Matrix crossover(Matrix partner)
    {
        Matrix child = new Matrix(rows, cols);

        //pick a random point in the matrix
        int randC = Random.Range(0, cols);
        int randR = Random.Range(0, rows);

        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            if (i < randR || i == randR && j <= randC)
                //if before the random point then copy from this matrix
                child.matrix[i, j] = matrix[i, j];
            else
                //if after the random point then copy from the partner matrix
                child.matrix[i, j] = partner.matrix[i, j];

        return child;
    }

    //sigmoid activation function
    float sigmoid(float x)
    {
        float y = (float) (1 / (1 + Math.Pow((float) Math.E, -x)));
        return y;
    }

    //returns an array which represents this matrix
    public float[] toArray()
    {
        float[] arr = new float[rows * cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                arr[j + i * cols] = matrix[i, j];
            }
        }

        return arr;
    }

    public string output()
    {
        string line = "";

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                line += matrix[i, j] + "  ";
            }

            line += "\n";
        }

        return line;
    }
}