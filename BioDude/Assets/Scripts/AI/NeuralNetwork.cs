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
    public Text outputPanelNN;
    int iNodes;
    int hNodes;
    int oNodes;

    Matrix whi; //matrix containing weights between the input nodes and the hidden nodes
    // Matrix whh; //matrix containing weights between the hidden nodes and the second layer hidden nodes
    Matrix woh; //matrix containing weights between the second hidden layer nodes and the output nodes
//    Matrix woi; //matrix containing weights between the input and the output nodes

    public NeuralNetwork(int inputCount, int hiddenCount, int outputCount)
    {
        outputPanelNN = GameObject.FindGameObjectWithTag("OutputNN").transform.GetComponent<Text>();

        iNodes = inputCount;
        hNodes = hiddenCount;
        oNodes = outputCount;

//        woi = new Matrix(oNodes, iNodes + 1);
        whi = new Matrix(hNodes, iNodes + 1);
        woh = new Matrix(oNodes, hNodes + 1);

//        woi.randomize();
        whi.randomize();
        woh.randomize();
    }

    // for cloning
    private NeuralNetwork(int inputCount, int hiddenCount, int outputCount, Matrix[] weights)
    {
        outputPanelNN = GameObject.FindGameObjectWithTag("OutputNN").transform.GetComponent<Text>();

        iNodes = inputCount;
        oNodes = outputCount;
        hNodes = hiddenCount;

//        woi = weights[0];
        whi = weights[0];
        woh = weights[1];
    }

    public NeuralNetwork crossover(NeuralNetwork partner)
    {
        return new NeuralNetwork(iNodes, hNodes, oNodes)
        {
//            woi = woi.crossover(partner.woi),
            whi = whi.crossover(partner.whi),
            woh = woh.crossover(partner.woh)
        };
    }

    //mutation function for genetic algorithm
    public void mutate(float mr)
    {
        //mutates each weight matrix
//        woi.mutate(mr);
        whi.mutate(mr);
//        whh.mutate(mr);
        woh.mutate(mr);
    }

    public float[] output(float[] inputsArr)
    {
        //convert array to matrix
        Matrix inputs = Matrix.singleColumnMatrixFromArray(inputsArr);

        //add bias 
        Matrix inputsBias = inputs.addBias();
        
        //-----------------------calculate the guessed output

        //apply weights
        Matrix hiddenInputs = whi.dot(inputsBias);

        //pass through activation function(sigmoid)
        Matrix hiddenOutputs = hiddenInputs.activate();

        Matrix hiddenOutputsBias = hiddenOutputs.addBias();
//        Matrix outputInputs = woi.dot(inputsBias);
        Matrix outputInputs = woh.dot(hiddenOutputsBias);
        Matrix outputs = outputInputs.activate();

        outputPanelNN.text = outputs.output();

        //convert to an array and return
        return outputs.toArray();
    }

    public NeuralNetwork clone()
    {
        return new NeuralNetwork(iNodes, hNodes, oNodes, new[]{whi, woh});
    }
}