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
    public int finishedCountToMove;

    private Transform posFinish;
    private Transform posStart;
    private Transform agentsFolder;
    private RDAgent[] agents;
    private int generation;
    private int posChanges;

    private int agentCountCurrent;
    private int bestAgentIndex;
    private int finishedCount;

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

        Destroy(agentsFolder.GetChild(0).gameObject);

        startNewGeneration();
    }

    void startNewGeneration()
    {
        generation++;

        if (generation > 1) // don't need fitness or mutation on first gen
        {
            float fitnessSum = setBestDude();

            UpdateStatusText();

            // natural selection

            NeuralNetwork[] newBrains = new NeuralNetwork[agentCount]; //next generation of agents

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

            moveStartAndFinishPos();
            //activate agents
            for (int i = 0; i < agentCount; i++)
                agents[i].Revive();
        }

        agentCountCurrent = agentCount;
        for (int i = 0; i < agentCount; i++) // put agents into starting position
            agents[i].transform.position = posStart.position;
    }

    void UpdateStatusText()
    {
        string gen = generation.ToString();
        string pos = "\n" + posChanges;
        string cnt = "\n" + agents.Sum(x => (x.finished ? 1 : 0));
        string best = "\n" + bestAgentIndex;
        string fit = "\n" + agents[bestAgentIndex].fitness;
        string step = "\n" + agents[bestAgentIndex].stepCount;
        outputPanel.text = gen + pos + cnt + best + fit + step;
    }

    void moveStartAndFinishPos()
    {
        if (finishedCount >= finishedCountToMove)
        {
            var start = getRandomSpotOnFloor();
            var finish = getRandomSpotOnFloor();

            posStart.position = new Vector3(start.x, start.y, posStart.position.z);
            posFinish.position = new Vector3(finish.x, finish.y, posFinish.position.z);

            posChanges++;
        }

        finishedCount = 0;
    }

    Vector2 getRandomSpotOnFloor()
    {
        float x = 0, y = 0;
        bool onFloor = false;

        while (!onFloor)
        {
            x = Random.Range(-4.514f, 4.514f);
            y = Random.Range(-3.518f, 20.598f);

            if (!((x < -0.515f || x > 0.515f) && y > 9.513f))
                onFloor = true;
        }

        return new Vector2(x, y);
    }

    float setBestDude()
    {
        float max = 0;
        float sum = 0;
        for (int i = 0; i < agentCount; i++)
        {
            agents[i].calculateFitness();
            sum += agents[i].fitness;

            if (agents[i].fitness > max)
            {
                max = agents[i].fitness;
                bestAgentIndex = i;
            }
        }

        return sum;
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

        Debug.Log("Fail: fitness " + fitnessSum);
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

    /// <summary>
    /// ////////////////////////////// PUBLIC METHODS ////////////////////////
    /// </summary>
    public void agentDone(bool finished)
    {
        if (finished)
            finishedCount++;
        agentCountCurrent--;
        if (agentCountCurrent <= 0)
            startNewGeneration();
    }
}