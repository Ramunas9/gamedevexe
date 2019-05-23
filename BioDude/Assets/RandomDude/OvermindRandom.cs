using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable MemberCanBeMadeStatic.Local

public class OvermindRandom : MonoBehaviour
{
    public int agentCount;
    public int maxSteps;
    public Transform agentPrefab;
    public float mutationRate;

    private Transform posFinish;
    private Transform posStart;
    private Transform agentsFolder;
    private RDAgent[] agents;
    private int generation;

    private int agentCountCurrent;
    private int bestAgentIndex;

    private Text outputPanel;

    // Start is called before the first frame update
    void Start()
    {
        posFinish = GameObject.Find("PositionFinish").transform;
        posStart = GameObject.Find("PositionStart").transform;
        agentsFolder = GameObject.Find("Agents").transform;
        agents = new RDAgent[agentCount];

        outputPanel = GameObject.FindGameObjectWithTag("Output").transform.GetComponent<Text>();

        for (int i = 0; i < agentCount; i++)
        {
            agents[i] = Instantiate(agentPrefab, agentsFolder).GetComponent<RDAgent>();
        }

        startNewGeneration();
    }

    void startNewGeneration()
    {
        generation++;

        if (generation > 1) // don't need fitness or mutation on first gen
        {
            float fitnessSum = 0;
            foreach (RDAgent a in agents)
            {
                a.calculateFitness();
                fitnessSum += a.fitness;
            }

            UpdateStatusText(generation, bestAgentIndex);

            NeuralNetwork[] newBrains = new NeuralNetwork[agentCount]; //next generation of agents

            // natural selection
            setBestDude(); // find best agent and place it into the next gen
            newBrains[0] = agents[bestAgentIndex].brain.clone();

            for (int i = 1; i < agentCount; i++)
            {
                NeuralNetwork parent1 = selectRandomParent(fitnessSum);
                NeuralNetwork parent2 = selectRandomParent(fitnessSum);

                NeuralNetwork child = parent1.crossover(parent2);

                child.mutate(mutationRate);

                newBrains[i] = child;
            }

            for (int i = 0; i < agentCount; i++)
                agents[i].brain = newBrains[i].clone();

            //activate agents
            for (int i = 0; i < agentCount; i++)
                agents[i].Revive();
        }


        agentCountCurrent = agentCount;
        for (int i = 0; i < agentCount; i++) // put agents into starting position
            agents[i].transform.position = posStart.position;

        Debug.Log(agentCountCurrent);
    }

    void UpdateStatusText(int generation, int bestIndex)
    {
        string gen = generation.ToString();
        string cnt = "\n" + agents.Sum(x => (x.finished ? 1 : 0));
        string best = "\n" + bestIndex;
        string fit = "\n" + agents[bestIndex].fitness;
        string step = "\n" + agents[bestIndex].stepCount;
        outputPanel.text = gen + cnt + best + fit + step;
    }

    void setBestDude()
    {
        double max = 0;
        for (int i = 0; i < agentCount; i++)
        {
            if (agents[i].fitness > max)
            {
                max = agents[i].fitness;
                bestAgentIndex = i;
            }
        }

        if (agents[bestAgentIndex].finished) // if he finished set new maxSteps
//            maxSteps = agents[bestAgentIndex].stepCount;
            maxSteps = agents[bestAgentIndex].stepCount < 1 ? 1 : agents[bestAgentIndex].stepCount;
    }

    NeuralNetwork selectRandomParent(float fitnessSum)
    {
        float rand = Random.Range(0, fitnessSum);
        float sum = 0;

        for (int i = 0; i < agentCount; i++)
        {
            sum += agents[i].fitness;
            if (sum > rand)
                return agents[i].brain.clone();
        }

        Debug.Log("Fail: sum " + sum + ", rand " + rand + ", fitness " + fitnessSum);
        return agents[0].brain.clone();
    }

    int getRandParent(float fitnessSum)
    {
        float randParent = Random.Range(0, fitnessSum);
        float sum = 0;

        for (int i = 0; i < agentCount; i++)
        {
            sum += agents[i].fitness;
            if (sum > randParent)
            {
                // clone parent steps array
                //agents[i].steps.CopyTo(newsteps, 0); // doesnt work wtf
                //System.Array.Copy(agents[i].steps, parent, maxSteps);
                //parent = newsteps;
                return i;
            }
        }

        Debug.Log("YOU CAN'T REACH THIS");
        return 0;
    }

    private void Swap(RDAgent source, RDAgent destination)
    {
        int hp = source.hp;
        int stepCount = source.stepCount;
        NeuralNetwork brain = source.brain.clone();
        float fitness = source.fitness;

        source.hp = destination.hp;
        source.stepCount = destination.stepCount;
        source.brain = destination.brain.clone();
        source.fitness = destination.fitness;

        destination.hp = hp;
        destination.stepCount = stepCount;
        destination.brain = brain;
        destination.fitness = fitness;
    }

    /// <summary>
    /// ////////////////////////////// PUBLIC METHODS ////////////////////////
    /// </summary>
    public void agentDone()
    {
        agentCountCurrent--;
        if (agentCountCurrent <= 0)
        {
            startNewGeneration();
        }
    }
}