using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MySql.Data.MySqlClient;
using UnityEngine.UI;

public class SceneStart : MonoBehaviour
{
    private string account;
    GameObject accountText;
    GameObject totalTime;
    GameObject audioVolume;
    GameObject soccerPlayerCount;
    GameObject goalTime;
    public static bool isSave = false;
    void Start() {
        account = PlayerPrefs.GetString("account");
        accountText = GameObject.Find("Canvas/SettingPanel/account");
        totalTime = GameObject.Find("Canvas/SettingPanel/totalTime/Dropdown/Label");
        audioVolume = GameObject.Find("Canvas/SettingPanel/audioVolume/Slider");
        soccerPlayerCount = GameObject.Find("Canvas/SettingPanel/soccerPlayerCount/Dropdown/Label");
        goalTime = GameObject.Find("Canvas/SettingPanel/goalTime/Dropdown/Label");
    }
    public void onStart(int sceneNumber){
        SceneManager.LoadScene(sceneNumber);
    }

    public void setPersonNumber(int pn){
        PlayerPrefs.SetInt("PerNum",pn);
    }

    public void reloadScene(){
        Application.LoadLevel(Application.loadedLevel);
        Time.timeScale = 1;
    }

    public void visit(){
        PlayerPrefs.SetString("account","游客");
    }

    public void showSettingMenu(){
        if(GameObject.FindGameObjectWithTag("Setting").GetComponent<CanvasGroup>().alpha == 1){
            GameObject.FindGameObjectWithTag("Setting").GetComponent<CanvasGroup>().alpha = 0;
        }else{
            GameObject.FindGameObjectWithTag("Setting").GetComponent<CanvasGroup>().alpha = 1;
        }
        
        if(account != "游客"){
            querySetting();
        }else{
            getSetting();
        }
    }

    
    public void querySetting(){
        string connstr = "data source=localhost;database=hf;user id=root;password=123456789;pooling=false;charset=utf8";
        MySqlConnection conn = new MySqlConnection(connstr);
        System.String sql = "select * from setting where account=@name";
        MySqlCommand cmd = new MySqlCommand(sql,conn);
        cmd.Parameters.AddWithValue("name",account);
        conn.Open();
        MySqlDataReader msdr =  cmd.ExecuteReader();
        if(msdr.Read()){
            accountText.GetComponent<Text>().text = msdr["account"].ToString();
            totalTime.GetComponent<Text>().text = msdr["totalTime"].ToString();
            audioVolume.GetComponent<Slider>().value = float.Parse(msdr["audioVolume"].ToString());
            soccerPlayerCount.GetComponent<Text>().text = msdr["soccerPlayerCount"].ToString();
            goalTime.GetComponent<Text>().text = msdr["goalTime"].ToString();
        }else{
            defaultSetting();
        }
        conn.Close(); 
    }

    public void saveSetting(){
        isSave = true; 
        if(account == "游客"){
            
        }else{
            string connstr = "data source=localhost;database=hf;user id=root;password=123456789;pooling=false;charset=utf8";
            MySqlConnection conn = new MySqlConnection(connstr);
            System.String sql = "replace into setting values(@name,@t,@a,@s,@g)";
            MySqlCommand cmd = new MySqlCommand(sql,conn);
            cmd.Parameters.AddWithValue("name",account);
            cmd.Parameters.AddWithValue("t",float.Parse(totalTime.GetComponent<Text>().text.ToString()));
            cmd.Parameters.AddWithValue("a",audioVolume.GetComponent<Slider>().value);
            cmd.Parameters.AddWithValue("s",int.Parse(soccerPlayerCount.GetComponent<Text>().text.ToString()));
            cmd.Parameters.AddWithValue("g",float.Parse(goalTime.GetComponent<Text>().text.ToString()));
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close(); 
        }
        PlayerPrefs.SetString("totalTime",totalTime.GetComponent<Text>().text);
        PlayerPrefs.SetFloat("audioVolume",audioVolume.GetComponent<Slider>().value);
        PlayerPrefs.SetString("soccerPlayerCount",soccerPlayerCount.GetComponent<Text>().text);
        PlayerPrefs.SetString("goalTime",goalTime.GetComponent<Text>().text);
        
        GameObject.FindGameObjectWithTag("Setting").GetComponent<CanvasGroup>().alpha = 0;
    }

    public void defaultSetting(){
        totalTime.GetComponent<Text>().text = "4";
        audioVolume.GetComponent<Slider>().value = 1.0f;
        soccerPlayerCount.GetComponent<Text>().text = "5";
        goalTime.GetComponent<Text>().text = "1.5";
        PlayerPrefs.SetString("totalTime",totalTime.GetComponent<Text>().text);
        PlayerPrefs.SetFloat("audioVolume",audioVolume.GetComponent<Slider>().value);
        PlayerPrefs.SetString("soccerPlayerCount",soccerPlayerCount.GetComponent<Text>().text);
        PlayerPrefs.SetString("goalTime",goalTime.GetComponent<Text>().text);
    }

    public void getSetting(){
        if(!isSave){
            defaultSetting();
        }else{
            totalTime.GetComponent<Text>().text = PlayerPrefs.GetString("totalTime");
            audioVolume.GetComponent<Slider>().value =  PlayerPrefs.GetFloat("audioVolume");
            soccerPlayerCount.GetComponent<Text>().text = PlayerPrefs.GetString("soccerPlayerCount");
            goalTime.GetComponent<Text>().text = PlayerPrefs.GetString("goalTime");
        }
    }

    public void initSetting(){
        if(!isSave){
            showSettingMenu();
        }
    }

    public GameObject ControlImg;
    public void showControl(){
        if(ControlImg.GetComponent<CanvasGroup>().alpha == 1){
            ControlImg.GetComponent<CanvasGroup>().alpha = 0;
        }else{
            ControlImg.GetComponent<CanvasGroup>().alpha = 1;
        }
    }
}
