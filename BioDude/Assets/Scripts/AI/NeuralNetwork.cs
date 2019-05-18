using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    Neuron[] inputNodes;
    Neuron[] outputNodes;
    int inputCount = 0;
    int outputCount = 0;
    List<Neuron[]> network;
    /// <summary>
    /// create neural network
    /// </summary>
    /// <param name="sizes">sizes of all layers (input, output included)</param>
    /// <param name="bias">bias values for each neuron (output included)</param>
    /// <param name="weights">weights for each neuron for each connection to previous layer neurons (output included)</param>
    public NeuralNetwork(int[] sizes, List<float[]> bias, List<List<float[]>> weights)
    {
        network = new List<Neuron[]>();
        for(int i = 0; i < sizes.Length; i++)
        {
            network.Add(new Neuron[sizes[i]]);
            for (int j = 0; j < sizes[i]; j++)
            {
                if (i == 0) // inputs
                {
                    network[0][j] = new Neuron(bias[0][j], null, null, true);
                }
                else
                {
                    network[i][j] = new Neuron(bias[i][j], network[i-1], weights[i-1][j]);
                }
            }
        }
    }

    public float[] getDecision(float[] inputs)
    {
        if(inputs.Length != inputCount)
        {
            return null;
        }
        //set input
        for(int i = 0; i < inputs.Length; i++)
        {
            inputNodes[i].setInput(inputs[i]);
        }
        //calculate each layer
        for (int i = 1; i < network.Count; i++)
        {
            for (int j = 0; j < network[i].Length; j++)
            {
                network[i][j].calculateValues();
            }
        }
        //get results
        float[] result = new float[outputCount];
        for (int i = 0; i < outputNodes.Length; i++)
        {
            result[i] = outputNodes[i].getOutput();
        }
        return result;
    }
}
