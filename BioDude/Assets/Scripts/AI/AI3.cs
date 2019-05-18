using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    NeuralNetwork network;
    int[] sizes = { 2, 5, 5, 2};
    List<float[]> bias = new List<float[]>();
    float defBias = 0.2f;
    List<List<float[]>> weights = new List<List<float[]>>();
    float defWeights = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sizes.Length; i++) // each layer
        {
            float[] _bias = new float[sizes[i]];
            float[] _weights = new float[sizes[i - 1]];
            weights[i] = new List<float[]>(); 
            for (int j = 0; j < sizes[i]; j++)  // each neuron
            {
                _bias[j] = defBias;
                for (int k = 0; k < sizes[i-1]; k++) // each input for current neuron
                {
                    _weights[k] = defWeights; 
                }
                weights[i].Add(_weights);
            }
            bias.Add(_bias);
        }
        //creating neural network
        network = new NeuralNetwork(sizes, bias, weights);
    }

    // Update is called once per frame
    void Update()
    {
        //get input

        //get output
        
    }
}
