using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable MemberCanBeMadeStatic.Local

public class NeuralNetwork
{
    public Text outputPanel;
    int iNodes;
//    int hNodes;
    int oNodes;

    // Matrix whi; //matrix containing weights between the input nodes and the hidden nodes
    // Matrix whh; //matrix containing weights between the hidden nodes and the second layer hidden nodes
    // Matrix woh; //matrix containing weights between the second hidden layer nodes and the output nodes
    Matrix woi; //matrix containing weights between the input and the output nodes

    public NeuralNetwork(int inputs, int outputCount)
    {
        outputPanel = GameObject.FindGameObjectWithTag("Output").transform.GetComponent<Text>();

        iNodes = inputs;
        oNodes = outputCount;
//        hNodes = hiddenCount;

        woi = new Matrix(oNodes, iNodes + 1);

        woi.randomize();
    }

    // for cloning
    private NeuralNetwork(int inputs, int outputCount, Matrix weights)
    {
        outputPanel = GameObject.FindGameObjectWithTag("Output").transform.GetComponent<Text>();

        iNodes = inputs;
        oNodes = outputCount;
//        hNodes = hiddenCount;

        woi = weights;
    }

    //mutation function for genetic algorithm
    public void mutate(float mr)
    {
        //mutates each weight matrix
//        whi.mutate(mr);
//        whh.mutate(mr);
//        woh.mutate(mr);
        woi.mutate(mr);
    }

    public float[] output(float[] inputsArr)
    {
        //convert array to matrix
        Matrix inputs = Matrix.singleColumnMatrixFromArray(inputsArr);

        //add bias 
        Matrix inputsBias = inputs.addBias();
        
        //-----------------------calculate the guessed output
        
        //apply weights
        Matrix outputInputs = woi.dot(inputsBias);
        //pass through activation function(sigmoid)
        Matrix outputs = outputInputs.activate();

        outputPanel.text = outputs.output();

        //convert to an array and return
        return outputs.toArray();
    }

    public NeuralNetwork clone()
    {
        return new NeuralNetwork(iNodes, oNodes, woi);

    }
}