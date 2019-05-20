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

    [HideInInspector] public bool isBest = false;
    public bool dead = false;
    public int stepCount { get; private set; }
    public bool finished { get; private set; }
    public int[] steps { get; private set; }

    private OvermindRandom overmind;
    private NeuralNetwork brain;

    private float[] decision;

    public float fitness { get; private set; }
    private int stepCountMax = 0;

    private string posFinishTag = "PositionFinish";
    private Vector3 posFinish;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        overmind = GameObject.FindGameObjectWithTag("Overmind").GetComponent<OvermindRandom>();
        posFinish = GameObject.FindGameObjectWithTag(posFinishTag).transform.position;
        stepCount = 0;
        stepCountMax = overmind.maxSteps;
        steps = new int[stepCountMax];
        randomizeSteps();

        brain = new NeuralNetwork(9, 4);

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
        float[] vision = new float[9];

        for (int i = 0; i < 8; i++)
        {
            vision[i] = lookInDirection(i);
        }

        vision[8] = Vector3.Distance(transform.position, posFinish);
        Debug.DrawLine(transform.position, posFinish, Color.blue);

        return vision;
    }

    // casts ray in direction by index and returns distance to wall if in vision distance or -1 otherwise.
    float lookInDirection(int directionIndex)
    {
        int angle;

        switch (directionIndex)
        {
            case 0:
                angle = 0;
                break;
            case 1:
                angle = 45;
                break;
            case 2:
                angle = 90;
                break;
            case 3:
                angle = 135;
                break;
            case 4:
                angle = 180;
                break;
            case 5:
                angle = 225;
                break;
            case 6:
                angle = 270;
                break;
            case 7:
                angle = 315;
                break;
            default:
                angle = 0;
                break;
        }

        Vector3 direction = Quaternion.Euler(0, 0, angle) * new Vector3(0, 1);
        direction = new Vector2(direction.x, direction.y);

        // Bit shift the index of the layer (17) to get a bit mask
        int layerMaskWall = 1 << 17;
//        layerMaskWall = ~layerMaskWall;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionDistance, layerMaskWall);

        Debug.DrawRay(transform.position, direction * visionDistance, Color.white);

        if (hit.collider != null)
        {
            Debug.DrawRay(transform.position, direction * hit.distance, Color.red);
            return hit.distance;
        }

        return -1;
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

        for (int i = 0; i < decision.Length; i++)
        {
            Debug.DrawRay(transform.position, translateIndexToDirection(i) * decision[i] * visionDistance,
                maxIndex == i ? Color.green : Color.yellow);
        }

        return maxIndex;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(col.gameObject.tag);
        if (col.gameObject.tag != posFinishTag)
        {
            hp--;
            if (hp <= 0)
            {
                dead = true;
                overmind.agentDone();
            }
        }
        else
        {
            finished = true;
            calculateFitness();
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

    void randomizeSteps()
    {
        for (int i = 0; i < stepCountMax; i++)
        {
            steps[i] = Random.Range(0, 4); // get random direction index
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
            fitness = 1.0f / 16.0f + 10000.0f / (Mathf.Pow(stepCount, 2));
        }
        else
        {
            //if the dot didn't reach the goal then the fitness is based on how close it is to the goal
            float distanceToGoal = Vector3.Distance(transform.position, posFinish);
            fitness = 1.0f / (Mathf.Pow(distanceToGoal, 2));
        }
    }

    public void Revive()
    {
        stepCount = 0;
        stepCountMax = steps.Length;
        dead = false;
        finished = false;
    }

    public void cloneSteps(int[] stepsToClone)
    {
        System.Array.Copy(stepsToClone, steps, stepsToClone.Length);
    }

    public void mutate(float mutationRate)
    {
        brain.mutate(mutationRate);
    }
}