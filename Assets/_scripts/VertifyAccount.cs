using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using UnityEngine.SceneManagement;

public class VertifyAccount : MonoBehaviour
{
    public InputField account;
    public InputField password;
    public Text errorText;
    public void vertify(){
        errorText.text = "";
        try{
            string name = test(account.text,password.text);
            if(name != null){
                SceneManager.LoadScene(1);
                Debug.Log("登录成功");
            }else{
                Debug.Log("账号或密码错误");
                errorText.text = "账号或密码错误!";
            }
            PlayerPrefs.SetString("account",name);
        }catch(Exception){
            errorText.text = "连接失败！！！";
        }
    }

    public static string test(string acc,string pas){
        string connstr = "data source=localhost;database=hf;user id=root;password=123456789;pooling=false;charset=utf8";
        MySqlConnection conn = new MySqlConnection(connstr);
        string sql = "select account from user where account=@a and password=@p";
        MySqlCommand cmd = new MySqlCommand(sql,conn);
        cmd.Parameters.AddWithValue("a",acc);
        cmd.Parameters.AddWithValue("p",pas);
        conn.Open();
        string name = null;
        try{
            name = cmd.ExecuteScalar().ToString();
        }catch(Exception){}
        conn.Close();        
        return name;
    }
}
