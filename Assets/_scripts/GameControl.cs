using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;


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
    private string account;
    public static bool isChangeGoal = false;

    void Start()
    {
        setScoreText(actUtils.Team.None);
        multi = getMulti();
        ballTran = GameObject.FindGameObjectWithTag("Ball").transform;
        ballRig = ballTran.GetComponent<Rigidbody>();
        utils = new actUtils(ballTran);
        isDualPlay = PlayerPrefs.GetInt("PerNum");
        account = PlayerPrefs.GetString("account");
        if(account == null||account==""){
            account = "游客";
        }
        
        totalTime = float.Parse(PlayerPrefs.GetString("totalTime"));
        count = int.Parse(PlayerPrefs.GetString("soccerPlayerCount"));
        goalTimeSet = float.Parse(PlayerPrefs.GetString("goalTime"));
        goalTime = goalTimeSet;

        start(actUtils.Team.Blue,isChangeGoal,isDualPlay);
    }

    private actUtils.Team teamTest;
    private bool timeChange = true;
    private bool gameOver = false;
    private bool recordGoal = true;
    void Update()
    {
        setAudioVolume(PlayerPrefs.GetFloat("audioVolume"));
        if(timeChange){
            changeTime();
        }
        if(gameOver){
            if(Input.GetKey(KeyCode.R)){
                Application.LoadLevel(Application.loadedLevel);
            }
        }
        if(recordGoal&&!gameOver){
            setScoreText(teamTest);
            teamTest = isGoal();
        }
        if(teamTest!=actUtils.Team.None||goalTime != goalTimeSet){
            goalTime -= Time.deltaTime;
            recordGoal = false;
        }
        if(goalTime < 0){
            restart(teamTest,isChangeGoal);
            goalTime = goalTimeSet;
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            showMenu();
        }
    }

    void PlayerIns(GameObject go,float xLimit1,float xLimit2){
        playerPosIns.z = UnityEngine.Random.Range(-limitPos.z,limitPos.z);
        playerPosIns.x = UnityEngine.Random.Range(xLimit1,xLimit2);
        Instantiate(go,playerPosIns,rotation);
    }

    private float xBlueLimit1,xBlueLimit2,xRedLimit1,xRedLimit2;
    void start(actUtils.Team team,bool isChange,int isDualPlay){
        Debug.Log(team+" "+isChange+" "+isDualPlay);
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

    private float goalPos = 15.7f;
    private actUtils.Team leftTeam = actUtils.Team.Blue;
    private actUtils.Team rightTeam = actUtils.Team.Red;
    private void changeGoal(){
        leftTeam = actUtils.Team.Red;
        rightTeam = actUtils.Team.Blue;
    }
    public actUtils.Team isGoal(){
        if(ballTran.position.x>goalPos){
            return rightTeam;
        }else if(ballTran.position.x<-goalPos){
            return leftTeam;
        }
        return actUtils.Team.None;
    }
    
    public float getMulti(){
        return 90 / totalTime;
    }
    private int minute = 0;
    private int second = 0;
    private float time1;
    void changeTime(){
        time1 += Time.deltaTime;
        second = System.Convert.ToInt32(multi*time1);
        if(second >= 60){
            minute += 1;
            second = 0;
            time1 = 0;
        }
        if(minute==45&&second==0){
            isChangeGoal = true;
            restart(actUtils.Team.Red,isChangeGoal);
            changeGoal();
        }else if(minute==90&&second==0){
            gameoverText.GetComponent<CanvasGroup>().alpha = 1;
            Invoke("DestroyAll",2.0f);
            timeChange = false;
            gameOver = true;
            recordScore(account,BlueScore,RedScore);
            Debug.Log("比分已记录");
            isChangeGoal = false;
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

    public void showMenu(){
        if(GameObject.FindWithTag("Record").GetComponent<CanvasGroup>().alpha == 1){
            GameObject.FindWithTag("Record").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.FindWithTag("Record").GetComponent<Image>().raycastTarget = false;
        }
        if(GameObject.FindWithTag("Setting").GetComponent<CanvasGroup>().alpha == 1){
            GameObject.FindWithTag("Setting").GetComponent<CanvasGroup>().alpha = 0;
        }
        if(GameObject.FindWithTag("Control").GetComponent<CanvasGroup>().alpha == 1){
            GameObject.FindWithTag("Control").GetComponent<CanvasGroup>().alpha = 0;
        }
        if(menu.GetComponent<CanvasGroup>().alpha == 1){
            Time.timeScale = 1;
            GameObject.FindWithTag("mask").GetComponent<Image>().raycastTarget = true;
            menu.GetComponent<CanvasGroup>().alpha = 0;
        }else{
            Time.timeScale = 0;
            GameObject.FindWithTag("mask").GetComponent<Image>().raycastTarget = false;
            menu.GetComponent<CanvasGroup>().alpha = 1;
        }
        
    }

    void recordScore(string account,int blue,int red){
        string connstr = "data source=localhost;database=hf;user id=root;password=123456789;pooling=false;charset=utf8";
        MySqlConnection conn = new MySqlConnection(connstr);
        string sql = "insert into record values(@a,@b,@r,@t,@p)";
        MySqlCommand cmd = new MySqlCommand(sql,conn);
        cmd.Parameters.AddWithValue("a",account);
        cmd.Parameters.AddWithValue("b",blue);
        cmd.Parameters.AddWithValue("r",red);
        cmd.Parameters.AddWithValue("t",DateTime.Now);
        cmd.Parameters.AddWithValue("p",isDualPlay);
        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    public void showRecordScoreMenu(){
        showMenu();
        Time.timeScale = 0;
        GameObject.FindWithTag("Record").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.FindWithTag("Record").GetComponent<Image>().raycastTarget = true;
        GameObject.FindWithTag("mask").GetComponent<Image>().raycastTarget = false;
        showRecordScore();
    }
    public GameObject Row_Prefab;
    void showRecordScore(){
        DestroyByTag("RecordRow");
        string connstr = "data source=localhost;database=hf;user id=root;password=123456789;pooling=false;charset=utf8";
        MySqlConnection conn = new MySqlConnection(connstr);
        String sql2 = "select * from record where account=@name";
        MySqlCommand cmd = new MySqlCommand(sql2,conn);
        cmd.Parameters.AddWithValue("name",account);
        conn.Open();
        MySqlDataReader msdr =  cmd.ExecuteReader();
        int i = 1;
        while(msdr.Read()){
            GameObject table = GameObject.Find("Canvas/Panel/table");
            GameObject row = GameObject.Instantiate(Row_Prefab, table.transform.position, table.transform.rotation) as GameObject;
            row.name = "row"+(i++);
            row.transform.SetParent(table.transform);
            row.transform.localScale = Vector3.one;
            row.transform.Find("account").GetComponent<Text>().text = msdr["account"].ToString();
            row.transform.Find("score").GetComponent<Text>().text = msdr["BLue"].ToString()+":"+msdr["Red"].ToString();
            row.transform.Find("time").GetComponent<Text>().text = msdr["time"].ToString();
            row.transform.Find("PerNum").GetComponent<Text>().text = msdr["PerNum"].ToString();
        }
        conn.Close(); 
    }

    public void setAudioVolume(float value){
        GetComponent<AudioSource>().volume = value;
    }

}
