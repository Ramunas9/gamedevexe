using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Matrix : MonoBehaviour
{
    private int rows;
    private int cols;
    private double[,] matrix;
    private Random rand;

    public Matrix(int r, int c)
    {
        rows = r;
        cols = c;
        matrix = new double[rows, cols];

        rand = new Random();
    }

    public void randomize()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = rand.Next(0, 200000) / 100000 - 1;
            }
        }
    }

    void output()
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