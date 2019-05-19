using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNet : MonoBehaviour
{
    int iNodes;
    int hNodes;
    int oNodes;

    // Matrix whi; //matrix containing weights between the input nodes and the hidden nodes
    // Matrix whh; //matrix containing weights between the hidden nodes and the second layer hidden nodes
    // Matrix woh; //matrix containing weights between the second hidden layer nodes and the output nodes
    Matrix woi; //matrix containing weights between the input and the output nodes

    public NeuralNet(int inputs, int hiddenCount, int outputCount)
    {
        iNodes = inputs;
        oNodes = outputCount;
        hNodes = hiddenCount;

        woi = new Matrix(oNodes, iNodes + 1);

        woi.randomize();
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
