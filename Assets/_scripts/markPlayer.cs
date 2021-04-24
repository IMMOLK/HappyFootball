using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class markPlayer : MonoBehaviour
{
    private GameObject player;
    void Update()
    {
        if(this.tag == "BlueMark"){
            player = GameObject.FindGameObjectWithTag("BluePlayer");
        }else{
            player = GameObject.FindGameObjectWithTag("RedPlayer");
        }
        Vector3 pos = player.transform.position;
        pos.y = 2.5f;
        this.transform.position = pos;
    }
}
