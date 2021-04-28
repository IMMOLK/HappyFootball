using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public GameObject blueP;
    public GameObject redP;
    public GameObject blueMark;
    public GameObject redMark;
    public GameObject ball;
    public int count;
    public float xLimit1;
    public float xLimit2;
    public Vector3 limitPos;
    public int isDualPlay;
    public float goalTimeSet = 1.5f;
    public Text timeText;
    public Text scoreText;
    public float totalTime;
    public GameObject menu;
    public GameObject gameoverText;
    private float goalTime;
    private Vector3 playerPos = Vector3.zero;
    private Vector3 playerPosIns;
    private Quaternion rotation = Quaternion.identity;
    private Transform ballTran;
    private Rigidbody ballRig;
    private actUtils utils;
    private float multi;


    void Start()
    {
        setScoreText(actUtils.Team.None);
        multi = 90.0f / totalTime ;
        goalTime = goalTimeSet;
        ballTran = GameObject.FindGameObjectWithTag("Ball").transform;
        ballRig = ballTran.GetComponent<Rigidbody>();
        utils = new actUtils(ballTran);
        isDualPlay = PlayerPrefs.GetInt("PerNum");
        start(actUtils.Team.Blue,false,isDualPlay);
    }

    private actUtils.Team teamTest;
    private bool timeChange = true;
    private bool gameOver = false;
    private bool recordGoal = true;
    void Update()
    {
        if(timeChange){
            changeTime();
        }
        if(gameOver){
            if(Input.GetKey(KeyCode.R)){
                Application.LoadLevel(Application.loadedLevel);
            }
        }
        if(recordGoal){
            setScoreText(teamTest);
            teamTest = isGoal();
        }
        if(teamTest!=actUtils.Team.None||goalTime != goalTimeSet){
            goalTime -= Time.deltaTime;
            recordGoal = false;
        }
        if(goalTime <= 0){
            restart(teamTest,false);
            goalTime = goalTimeSet;
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            showMenu();
        }
    }

    void PlayerIns(GameObject go,float xLimit1,float xLimit2){
        playerPosIns.z = Random.Range(-limitPos.z,limitPos.z);
        playerPosIns.x = Random.Range(xLimit1,xLimit2);
        Instantiate(go,playerPosIns,rotation);
    }

    private float xBlueLimit1,xBlueLimit2,xRedLimit1,xRedLimit2;
    void start(actUtils.Team team,bool isChange,int isDualPlay){
        if(isChange){
            xBlueLimit1 = xLimit1;
            xBlueLimit2 = xLimit2;
            xRedLimit1 = -xLimit2;
            xRedLimit2 = -xLimit1;
        }else{
            xRedLimit1 = xLimit1;
            xRedLimit2 = xLimit2;
            xBlueLimit1 = -xLimit2;
            xBlueLimit2 = -xLimit1;
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
        Instantiate(blueMark,playerPos,rotation);
        if(isDualPlay == 2){
            searchPlayer("Red","RedPlayer"); 
            Instantiate(redMark,playerPos,rotation);
        }
    }

    void searchPlayer(string tag1 , string tag2){       
        Transform Player = utils.queryClosest(tag1,ballTran.position);
        Player.tag = tag2;
    }

    void DestroyByTag(string tag){
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject go in gos){
            DestroyImmediate(go);
        }
    }

    void DestroyAll(){
        DestroyByTag("BlueMark");
        DestroyByTag("BluePlayer");
        DestroyByTag("RedMark");
        DestroyByTag("RedPlayer");
        DestroyByTag("Blue");
        DestroyByTag("Red");
    }

    void restart(actUtils.Team team,bool isChange){
        DestroyAll();
        ballRig.velocity = Vector3.zero;
        ballRig.constraints = RigidbodyConstraints.FreezeAll;
        ballRig.constraints = RigidbodyConstraints.None;
        ballTran.position = playerPos;
        start(team,isChange,isDualPlay);
        recordGoal = true;
    }

    public actUtils.Team isGoal(){
        if(ballTran.position.x>15.7f){
            return actUtils.Team.Red;
        }else if(ballTran.position.x<-15.7f){
            return actUtils.Team.Blue;
        }
        return actUtils.Team.None;
    }
    
    private int minute = 0;
    private int second = 0;
    private float time1;
    void changeTime(){
        time1 += Time.deltaTime;
        second = System.Convert.ToInt32(multi*time1);
        if(second == 60){
            minute += 1;
            second = 0;
            time1 = 0;
        }
        if(minute==45&&second==0){
            restart(actUtils.Team.Blue,true);
        }else if(minute==90&&second==0){
            gameoverText.GetComponent<CanvasGroup>().alpha = 1;
            Invoke("DestroyAll",2.0f);
            timeChange = false;
            gameOver = true;
        }
        timeText.text = minute.ToString().PadLeft(2,'0')+":"+second.ToString().PadLeft(2,'0');
    }

    private int BlueScore = 0;
    private int RedScore = 0;
    void setScoreText(actUtils.Team team){
        if(team == actUtils.Team.Blue){
            RedScore += 1;
        }else if(team == actUtils.Team.Red){
            BlueScore += 1;
        }
        scoreText.text = "BLUE   "+BlueScore+" : "+RedScore+"   RED";
    }

    void showMenu(){
        if(menu.GetComponent<CanvasGroup>().alpha == 1){
            menu.GetComponent<CanvasGroup>().alpha = 0;
        }else{
            menu.GetComponent<CanvasGroup>().alpha = 1;
        }
        
    }

}
