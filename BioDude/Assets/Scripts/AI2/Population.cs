using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable MemberCanBeMadeStatic.Local

public class Population : MonoBehaviour
{
    public int maxSteps = 10000;
    public const int playerCount = 100;
    public Transform playerPrefab;
    public float mutationRate = 0.1f;

    private Transform posFinish;
    private Transform posStart;
    private Transform populationFolder;
    private player[] players;
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
        populationFolder = GameObject.Find("Population").transform;
        players = new player[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            players[i] = Instantiate(playerPrefab, populationFolder).GetComponent<player>();
        }
    }

    void FixedUpdate()
    {
        Debug.Log(stepCount++);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void agentDone()
    {
        agentCountCurrent--;
        if(agentCountCurrent <= 0)
        {
            startNewGeneration();
        }
    }
}
