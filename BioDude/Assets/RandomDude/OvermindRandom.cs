using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OvermindRandom : MonoBehaviour
{
    public int agentCount = 1;
    public int maxSteps = 10000;
    public Transform agentPrefab;
    public float mutationRate = 0.1f;

    private Transform posFinish;
    private Transform posStart;
    private Transform agentsFolder;
    private RDAgent[] agents;
    private int generation = 0;

    private int agentCountCurrent;
    private float fitnessSum = 0;
    private int bestAgentIndex = 0;

    private int stepCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        posFinish = GameObject.Find("PositionFinish").transform;
        posStart = GameObject.Find("PositionStart").transform;
        agentsFolder = GameObject.Find("Agents").transform;
        agents = new RDAgent[agentCount];

        for (int i = 0; i < agentCount; i++)
        {
            agents[i] = Instantiate(agentPrefab, agentsFolder).GetComponent<RDAgent>();
        }

        startNewGeneration();
    }

    void FixedUpdate()
    {
        Debug.Log(stepCount++);
    }

    void startNewGeneration()
    {
        stepCount = 0;
        generation++;

        if (generation > 1) // don't need fitness or mutation on first gen
        {
            foreach (RDAgent a in agents)
            {
                a.calculateFitness();
                fitnessSum += a.fitness;
            }

            // natural selection
            setBestDot(); // find best agent and place it into the next gen
            if(bestAgentIndex != 0) // put the best agent in first position
            {
                RDAgent temp = agents[0];
                agents[0] = agents[bestAgentIndex];
                agents[bestAgentIndex] = temp;
            }

            // create parents off unmodified agents
            int[][] parents = new int[agentCount][];
            for (int i = 1; i < agentCount; i++)
            {
                //parents[i] = getRandParent();
                //getRandParent(ref parents[i]);
                parents[i] = new int[maxSteps];
                int index = getRandParent();
                System.Array.Copy(agents[index].steps, parents[i], maxSteps);
                Debug.Log("agent: " + agents[index].steps[0]);
                Debug.Log("parent: " + parents[i][0]);
            }
            Debug.Log("parent AFTER: " + parents[1][0]);

            // get parent steps, mutate them and assign to agent
            for (int i = 1; i < agentCount; i++) 
            {
                agents[i].cloneSteps(parents[i]);
                agents[i].mutate(mutationRate);
            }

            //activate agents
            for (int i = 0; i < agentCount; i++)
                agents[i].Revive();
        }


        agentCountCurrent = agentCount;
        for (int i = 0; i < agentCount; i++) // put agents into starting position
            agents[i].transform.position = posStart.position;


        Debug.Log(agentCountCurrent);
    }

    void setBestDot()
    {
        double max = 0;
        for (int i = 0; i < agentCount; i++)
        {
            if(agents[i].fitness > max)
            {
                max = agents[i].fitness;
                bestAgentIndex = i;
            }
        }
        if(agents[bestAgentIndex].finished) // if he finished set new maxSteps
            maxSteps = agents[0].stepCount;
    }

    int getRandParent()
    {
        float randParent = Random.Range(0, fitnessSum);
        float sum = 0;

        for (int i = 0; i < agentCount; i++)
        {
            sum += agents[i].fitness;
            if(sum > randParent)
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
    public void agentDone()
    {
        agentCountCurrent--;
        if(agentCountCurrent <= 0)
        {
            startNewGeneration();
        }
    }
}
