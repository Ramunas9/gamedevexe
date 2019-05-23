using System;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable MemberCanBeMadeStatic.Local

public class RDAgent : MonoBehaviour
{
    public int hp;
    public float moveForce;
    public int visionDistance;

    [HideInInspector] public bool dead = false;
    public int stepCount { get; set; }
    public bool finished { get; private set; }

    private OvermindRandom overmind;
    public NeuralNetwork brain;

    private float[] decision;

    public float fitness { get; set; }
    private int stepCountMax = 0;

    private string posFinishTag = "PositionFinish";
    private Vector3 posFinish;
    private Rigidbody2D rb;

    private LineRenderer[] staticLines;
    private LineRenderer[] visionLines;
    private LineRenderer[] decisionLines;

    private const float staticLinesZ = -9.1f;
    private const float visionLinesZ = -9.4f;
    private const float decisionLinesZ = -9.3f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        overmind = GameObject.FindGameObjectWithTag("Overmind").GetComponent<OvermindRandom>();
        posFinish = GameObject.FindGameObjectWithTag(posFinishTag).transform.position;
        stepCount = 0;
        stepCountMax = overmind.maxSteps;

        brain = new NeuralNetwork(9, 0, 4);

        staticLines = transform.GetChild(1).GetComponentsInChildren<LineRenderer>();
        visionLines = transform.GetChild(2).GetComponentsInChildren<LineRenderer>();
        decisionLines = transform.GetChild(3).GetComponentsInChildren<LineRenderer>();

        Revive();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead && !finished)
        {
            if (stepCount < stepCountMax)
            {
                float[] vision = look();
                int direction = decideDirection(vision);

                rb.AddForce(translateIndexToDirection(direction) * moveForce); // take a step from steps array
                stepCount++;
            }
            else
            {
                Debug.Log("DEAD");
                dead = true;
                overmind.agentDone();
            }
        }
    }

    float[] look()
    {
        // Bit shift the index of the layer (17) to get a bit mask
        int layerMaskWall = 1 << 17;
        float[] vision = new float[9];

        Vector3 linePosStaticStart = transform.position;
        Vector3 linePosVisionStart = transform.position;
        Vector3 linePosVisionEnd = transform.position;
        linePosStaticStart.z = staticLinesZ;
        linePosVisionStart.z = visionLinesZ;

        for (int i = 0; i < 8; i++)
        {
            int angle = 45 * i;

            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.up;
            direction = new Vector2(direction.x, direction.y);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionDistance, layerMaskWall);

            Vector3 linePosStaticEnd = transform.position + direction * visionDistance;

            if (hit.collider != null)
                linePosVisionEnd = hit.point;

            linePosStaticEnd.z = staticLinesZ;
            linePosVisionEnd.z = visionLinesZ;
            staticLines[i].SetPositions(new[] {linePosStaticStart, linePosStaticEnd});
            visionLines[i].SetPositions(new[] {linePosVisionStart, linePosVisionEnd});

            vision[i] = visionDistance - hit.distance;
        }

        vision[8] = Vector3.Distance(transform.position, posFinish);
        visionLines[8].SetPositions(new[] {linePosVisionStart, new Vector3(posFinish.x, posFinish.y, visionLinesZ)});

        return vision;
    }

    int decideDirection(float[] vision)
    {
        decision = brain.output(vision);

        //get the maximum value in the output array and use this as the decision on which direction to go
        float max = 0;
        int maxIndex = 0;
        for (int i = 0; i < decision.Length; i++)
        {
            if (max < decision[i])
            {
                max = decision[i];
                maxIndex = i;
            }
        }

        Vector3 linePosDecisionStart = transform.position;
        linePosDecisionStart.z = decisionLinesZ;
        for (int i = 0; i < decision.Length; i++)
        {
//            decisionLines[i].material.color = maxIndex == i ? Color.green : Color.yellow;

            var end = translateIndexToDirection(i) * decision[i] * visionDistance;
            Vector3 linePosDecisionEnd = transform.position + new Vector3(end.x, end.y, decisionLinesZ);
            decisionLines[i].SetPositions(new[] {linePosDecisionStart, linePosDecisionEnd});
        }

        return maxIndex;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
//        Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag != posFinishTag)
        {
            hp--;
            if (hp > 0) return;

            foreach (var line in staticLines)
                line.SetPositions(new[] {Vector3.zero, Vector3.zero});
            foreach (var line in visionLines)
                line.SetPositions(new[] {Vector3.zero, Vector3.zero});
            foreach (var line in decisionLines)
                line.SetPositions(new[] {Vector3.zero, Vector3.zero});

            dead = true;
            rb.velocity = Vector2.zero;
            overmind.agentDone();
        }
        else
        {
            finished = true;
//            calculateFitness();
            overmind.agentDone();
        }
    }

    Vector2 translateIndexToDirection(int index)
    {
        switch (index)
        {
            case 1:
                return Vector2.right;
            case 2:
                return Vector2.down;
            case 3:
                return Vector2.left;
            default:
                return Vector2.up;
        }
    }

    /// <summary>
    /// ///////////////////////////////////// PUBLIC METHODS /////////////////////////////////////////////////
    /// </summary>
    public void calculateFitness()
    {
        if (finished)
        {
            //if the dot reached the goal then the fitness is based on the amount of steps it took to get there
            fitness = 1.0f + 10000.0f / Mathf.Pow(stepCount, 2);
        }
        else
        {
            //if the dot didn't reach the goal then the fitness is based on how close it is to the goal
            float distanceToGoal = Vector3.Distance(transform.position, posFinish);
            fitness = distanceToGoal <= 0 ? 1 : 1.0f / Mathf.Pow(distanceToGoal, 2);

            if (distanceToGoal <= 0 || fitness >= Mathf.Infinity)
            {
                Debug.Log(distanceToGoal + " FUCKED UP " + fitness);
            }

//            fitness = fitness >= Single.PositiveInfinity ? 0 : fitness;
        }
    }

    public void Revive()
    {
        hp = transform.GetComponent<RDAgent>().hp;
        stepCount = 0;
//        stepCountMax = steps.Length;
        dead = false;
        finished = false;
    }

    public void mutate(float mutationRate)
    {
        brain.mutate(mutationRate);
    }
}