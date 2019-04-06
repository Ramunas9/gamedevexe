﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RDAgent : MonoBehaviour
{
    public int hp = 1;
    public float moveForce = 10f;

    [HideInInspector]
    public bool isBest = false;
    public bool dead = false;
    public int stepCount { get; private set; }
    public bool finished { get; private set; }
    public int[] steps { get; private set; }

    private OvermindRandom overmind;

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
        Revive();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead && !finished)
        {
            if(stepCount < stepCountMax)
            {
                rb.AddForce(translateIndexToDirection(steps[stepCount]) * moveForce); // take a step from steps array
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

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(col.gameObject.tag);
        if(col.gameObject.tag != posFinishTag)
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
        switch (index) {
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
        {//if the dot reached the goal then the fitness is based on the amount of steps it took to get there
            fitness = 1.0f / 16.0f + 10000.0f / (Mathf.Pow(stepCount, 2));
        }
        else
        {//if the dot didn't reach the goal then the fitness is based on how close it is to the goal
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
        //int[] newsteps = new int[stepsToClone.Length];

        //steps = newsteps;
        ///*steps = (int[])*/stepsToClone.CopyTo(steps, 0);
        System.Array.Copy(stepsToClone, steps, stepsToClone.Length);
    }

    public void mutate(float mutationRate)
    {
        for (int i = 0; i < stepCountMax; i++)
        {
            if (Random.Range(0, 1.0f) < mutationRate)
            {
                steps[i] = Random.Range(0, 4);
            }
        }
    }
}