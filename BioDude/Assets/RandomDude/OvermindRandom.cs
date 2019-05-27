using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class OvermindRandom : MonoBehaviour
{
    public int agentCount;
    public int maxSteps;
    public Transform agentPrefab;
    public float mutationRate;
    private float fitnessSum;

    private Transform posFinish;
    private Transform posStart;
    private Transform agentsFolder;
    private RDAgent[] agents;
    private int generation;

    private int agentCountCurrent;
    private int bestAgentIndex;

    private Text outputPanel;
    private string filename;

    // Start is called before the first frame update
    void Start()
    {
        filename = string.Format("{0:yyyy-MM-dd}_{1}__result.csv", System.DateTime.Now, System.DateTime.Now.ToString("HH;mm;ss"));

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
            fitnessSum = setBestDude();

            UpdateStatusText();

            ResultsToFile();

            // natural selection
            if (bestAgentIndex != 0) // put the best agent in first position
                Swap(agents[0], agents[bestAgentIndex]);

            // create parents off unmodified agents
            int[][] parents = new int[agentCount][];
            for (int i = 1; i < agentCount; i++)
            {
                //parents[i] = getRandParent();
                //getRandParent(ref parents[i]);
                parents[i] = new int[maxSteps];
                Debug.Log("maxsteps: " + maxSteps);
                int index = getRandParent(fitnessSum);
                System.Array.Copy(agents[index].steps, parents[i], maxSteps);
                Debug.Log("agent: " + agents[index].steps[0]);
                Debug.Log("parent: " + parents[i][0]);
            }

            Debug.Log("parent AFTER: " + parents[1][0]);

            // get parent steps, mutate them and assign to agent
            for (int i = 1; i < agentCount; i++)
            {
                agents[i].cloneSteps(parents[i]);
                agents[i].mutate(mutationRate * (1 - agents.Sum(x => (x.finished ? 1 : 0) / agentCount)));
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

    void UpdateStatusText()
    {
        string gen = generation.ToString();
        string cnt = "\n" + agents.Sum(x => (x.finished ? 1 : 0));
        string best = "\n" + bestAgentIndex;
        string fit = "\n" + agents[bestAgentIndex].fitness;
        string step = "\n" + agents[bestAgentIndex].stepCount;
        outputPanel.text = gen + cnt + best + fit + step;
    }

    void ResultsToFile()
    {
        if (!File.Exists("Results/" + filename))
        {
            using (var sw = new StreamWriter("Results/" + filename, true))
            {
                sw.WriteLine(
                    "Generation,Agent Count,Mutation Rate,Best fitness,Avg. Fitness,Max. step count"
                    );
            }
        }

        using (var sw = new StreamWriter("Results/" + filename, true))
        {
            sw.WriteLine(string.Format(
                "{0},{1},{2},{3},{4},{5}", generation, agentCount, mutationRate,
                agents[bestAgentIndex].fitness, fitnessSum / agentCount, maxSteps
                ));
        }
    }

    float setBestDude()
    {
        float max = 0;
        float sum = 0;
        for (int i = 0; i < agentCount; i++)
        {
            agents[i].calculateFitness();
            sum += agents[i].fitness;
            Debug.Log(i + " -- " + agents[i].stepCount + " -- " + agents[i].fitness + " -- " + agents[i].finished);

            if (agents[i].fitness > max)
            {
                max = agents[i].fitness;
                bestAgentIndex = i;
            }
        }

        if (agents[bestAgentIndex].finished) // if he finished set new maxSteps
//            maxSteps = agents[bestAgentIndex].stepCount;
            maxSteps = agents[bestAgentIndex].stepCount < 1 ? maxSteps : agents[bestAgentIndex].stepCount;
        return sum;
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
        int[] steps = new int[source.steps.Length];
        System.Array.Copy(source.steps, steps, source.steps.Length);
        float fitness = source.fitness;

        source.hp = destination.hp;
        source.stepCount = destination.stepCount;
        System.Array.Copy(destination.steps, source.steps, source.steps.Length);
        source.fitness = destination.fitness;

        destination.hp = hp;
        destination.stepCount = stepCount;
        System.Array.Copy(steps, destination.steps, source.steps.Length);
        destination.fitness = fitness;
    }

    /// <summary>
    /// ////////////////////////////// PUBLIC METHODS ////////////////////////
    /// </summary>
    public void agentDone()
    {
        agentCountCurrent--;
        if (agentCountCurrent <= 0)
            startNewGeneration();
    }
}