using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public GameObject blueP;
    public GameObject redP;
    public int count;
    public float xLimit1;
    public float xLimit2;
    public Vector3 limitPos;
    public bool isDualPlay;
    private Vector3 playerPos = Vector3.zero;
    private Quaternion rotation = Quaternion.identity;
    private Transform ballTran;
    private actUtils utils;

    void Start()
    {
        ballTran = GameObject.FindGameObjectWithTag("Ball").transform;
        utils = new actUtils(ballTran);
        restart(actUtils.Team.Blue,false,true);
    }


    void Update()
    {
        
    }

    void PlayerIns(GameObject go,float xLimit1,float xLimit2){
        playerPos.z = Random.Range(-limitPos.z,limitPos.z);
        playerPos.x = Random.Range(xLimit1,xLimit2);
        Instantiate(go,playerPos,rotation);
    }

    private float xBlueLimit1,xBlueLimit2,xRedLimit1,xRedLimit2;
    void restart(actUtils.Team team,bool isChange,bool isDualPlay){
        if(isChange){
            xBlueLimit1 = xLimit1;
            xBlueLimit2 = xLimit2;
            xRedLimit1 = -xLimit1;
            xRedLimit2 = -xLimit2;
        }else{
            xRedLimit1 = xLimit1;
            xRedLimit2 = xLimit2;
            xBlueLimit1 = -xLimit1;
            xBlueLimit2 = -xLimit2;
        }

        switch(team){
            case actUtils.Team.Blue: Instantiate(blueP,playerPos,rotation); 
                                     PlayerIns(redP,xRedLimit1,xRedLimit2);  
                                     break;
            case actUtils.Team.Red:  Instantiate(redP,playerPos,rotation);
                                     PlayerIns(blueP,xBlueLimit1,xBlueLimit2); 
                                     break;
        }
        for(int i =0;i<count-1;i++){
            PlayerIns(blueP,xBlueLimit1,xBlueLimit2);
            PlayerIns(redP,xRedLimit1,xRedLimit2);
        }
        searchPlayer("Blue","BluePlayer");
        searchPlayer("Red","RedPlayer"); 
    }

    void searchPlayer(string tag1 , string tag2){       
        Transform bluePlayer = utils.queryClosest(tag1);
        bluePlayer.tag = tag2;
        bluePlayer.GetComponent<Play>().enabled = true;
    }

}
