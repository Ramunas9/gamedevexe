﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HazardTiles : MonoBehaviour
{


    public int damage = 1;              //this is main strenght measure of damage/slows/force
    public int damageMultiplyer = 1;    //this is damage multiplyer, intended to be used on higher level traps, so same damage value could be kept.
    public int damageDuration = 3;      //this is duration for post-tile-leave effects
    public int hazardId = 1;            //this is id of hazard tiles
    public int direction = 1;           //Directions: 1=up, 2=down, 3=left, 4=right

    public GameObject player;
    private PlayerHealthManager playerHealth;
    private PlayerMovement playerMovement;

    public int interval = 1;            //do something every x seconds
    private int timeInSeconds = 0;      //Total time in seconds
    private int lastTime = 0;           //last second on time counter
    private float defaultSpeed;         //default player speed
    private int tickUntil;              //used for timer duration
    private bool damaged = false;       //used for one time damage check

    private bool onTriggerStay2D = false;

    // Use this for initialization
    void Start()
    {
        playerHealth = player.GetComponent<PlayerHealthManager>();
        playerMovement = player.GetComponent<PlayerMovement>();
        InvokeRepeating("AddSecond", 1f, 1f);  //1s delay, repeat every 1s
        switch (hazardId)
        {
            case 3:
                InvokeRepeating("DamageOverTime", 1f, 1f);  //1s delay, repeat every 1s
                break;
            case 4:
                InvokeRepeating("SlowOverTime", 1f, 1f);  //1s delay, repeat every 1s
                break;
            default:
                break;
        }
        defaultSpeed = playerMovement.speed;
    }

    private void AddSecond()
    {
        timeInSeconds++;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LaunchTimer()
    {
        tickUntil = timeInSeconds + damageDuration;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            onTriggerStay2D = true;
            switch (hazardId)
            {
                case 3:
                    tickUntil = timeInSeconds;
                    break;
                case 5:
                    InstantDamage();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            onTriggerStay2D = false;
            LaunchTimer();
            switch (hazardId)
            {
                case 2:
                    ClearSlow();
                    break;
                case 3:
                    DamageOverTime();
                    break;
                case 5:
                    damaged = false;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (hazardId)
        {
            case 6:
                Conveyor(collision);
                break;
            default:
                if (collision.gameObject.tag == "Player")
                {
                    switch (hazardId)
                    {
                        default:
                            if (timeInSeconds % interval == 0 && lastTime != timeInSeconds)
                            {
                                lastTime = timeInSeconds;
                                switch (hazardId)
                                {
                                    case 4:
                                        LaunchTimer();
                                        break;
                                    default:
                                        break;
                                }
                                GetHazardById(hazardId);
                            }
                            break;
                    }
                }
                break;
        }
    }

    public void GetHazardById(int id)
    {
        switch (id)
        {
            case 1:
                Damage();
                break;
            case 2:
                Slow();
                break;
            case 3:
                Damage();
                DamageOverTime();
                break;
            case 4:
                SlowOverTime();
                break;
            default:
                break;
        }
    }

    private void Damage()
    {
        playerHealth.HurtPlayer(damage * damageMultiplyer);
    }
    private void Slow()
    {
        var speedSub = damage * damageMultiplyer;
        if (speedSub < defaultSpeed)
            playerMovement.speed = defaultSpeed - speedSub;
        else
        {
            playerMovement.speed = 1;
        }
    }
    private void ClearSlow()
    {
        playerMovement.speed = defaultSpeed;
    }
    private void DamageOverTime()
    {
        if (timeInSeconds % interval == 0)
            if (tickUntil >= timeInSeconds)
            {
                Damage();
            }
    }
    private void SlowOverTime()
    {
        if (timeInSeconds % interval == 0)
            if (tickUntil >= timeInSeconds)
            {
                Slow();
            }
            else;
        else
        {
            if (!onTriggerStay2D)
                ClearSlow();
        }
        if (tickUntil < timeInSeconds)
        {
            ClearSlow();
        }
    }
    private void InstantDamage()
    {
        if (!damaged)
            Damage();
        damaged = true;
    }
    private void Conveyor(Collider2D collision)
    {
        switch (direction)
        {
            case 1:
                collision.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0,1 * damage * damageMultiplyer));
                break;
            case 2:
                collision.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, -1 * damage * damageMultiplyer));
                break;
            case 3:
                collision.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(-1 * damage * damageMultiplyer, 0));
                break;
            case 4:
                collision.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(1 * damage * damageMultiplyer, 0));
                break;
            default:
                break;
        }
        
    }
}