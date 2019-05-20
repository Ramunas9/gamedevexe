using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPos : MonoBehaviour
{
    public string playerTag = "Agent";
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == playerTag)
        {
            col.gameObject.SetActive(false);
            //proccess finish
        }
    }
}
