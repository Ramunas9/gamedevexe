using System;
using UnityEngine;
using Random = System.Random;
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable MemberCanBeMadeStatic.Local

public class Matrix : MonoBehaviour
{
    private int rows;
    private int cols;
    private float[,] matrix;
    private Random rand;

    public Matrix(int r, int c)
    {
        rows = r;
        cols = c;
        matrix = new float[rows, cols];

        rand = new Random();
    }

    public void randomize()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = (float) (rand.NextDouble() * 2 - 1);
            }
        }
    }

    public Matrix dot(Matrix n)
    {
        Matrix result = new Matrix(rows, n.cols);

        if (cols == n.rows)
        {
            //for each spot in the new matrix
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < n.cols; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < cols; k++)
                    {
                        sum += matrix[i, k] * n.matrix[k, j];
                    }

                    result.matrix[i, j] = sum;
                }
            }
        }

        return result;
    }

    //Mutation function for genetic algorithm 
    public void mutate(float mutationRate)
    {
        //for each element in the matrix
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float randomNextDouble = (float) (rand.NextDouble() * 2 - 1);
                if (randomNextDouble < mutationRate)
                {
                    //if chosen to be mutated
//                    matrix[i, j] += randomGaussian()/5;//add a random value to it(can be negative)
                    matrix[i, j] += (float) (rand.NextDouble() * 2 - 1); //add a random value to it(can be negative)

                    //set the boundaries to 1 and -1
                    if (matrix[i, j] > 1)
                    {
                        matrix[i, j] = 1;
                    }

                    if (matrix[i, j] < -1)
                    {
                        matrix[i, j] = -1;
                    }
                }
            }
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

//---------------------------------------------------------------------------------------------------------------------------------------------------------    
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

    public void output()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                print(matrix[i, j] + "  ");
            }

            Console.WriteLine(" ");
        }


        Console.WriteLine();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}