using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron : MonoBehaviour
{
    private float[] weights;
    private float bias;
    private bool isRoot = false;
    private Neuron[] inputs;
    private float currentValue = 0;

    public Neuron(float bias, Neuron[] neurons, float[] weights, bool isRoot = false)
    {
        if (isRoot)
        {
            
        } else
        {
            this.bias = bias;
            this.inputs = neurons;
            this.weights = weights;
        }
    }

    public float getOutput()
    {
        return currentValue;
    }

    public void calculateValues()
    {
        if (isRoot)
        {
            currentValue = bias;
        }
        else
        {
            currentValue = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                currentValue += inputs[i].getOutput() * weights[i];
            }
            // < activationfunction goes here
            currentValue += bias;
        }
    }

    public void setInput(float input)
    {
        if (isRoot)
        {
            bias = input;
        }
    }
    
}
