using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using UnityEngine.SceneManagement;
using System;

public class Regist : MonoBehaviour
{
    public GameObject panel;
    public InputField account;
    public InputField password;
    public InputField repwd;
    public Text errorText;
    
    public void showMenu(){
        if(panel.GetComponent<CanvasGroup>().alpha == 0){
            panel.GetComponent<CanvasGroup>().alpha = 1;
        }else{
            closeMenu();
        }
    }

    public void closeMenu(){
        panel.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void regist(){
        if(account.text==""||password.text==""||repwd.text==""){
            errorText.text = "输入不得为空！!";
            return;
        }
        errorText.text = "";
        if(!repwd.text.Equals(password.text)){
            errorText.text = "两次密码输入不一致！!";
            return;
        }
        string name = VertifyAccount.test(account.text,password.text);
        if(name != null){
           errorText.text = "此账户已存在！!";
           return;
        }
        addUer();
        errorText.text = "注册成功!";
    }


    public void addUer(){
        string connstr = "data source=localhost;database=hf;user id=root;password=123456789;pooling=false;charset=utf8";
        MySqlConnection conn = new MySqlConnection(connstr);
        string sql = "insert into user values(@a,@p)";
        MySqlCommand cmd = new MySqlCommand(sql,conn);
        cmd.Parameters.AddWithValue("a",account.text);
        cmd.Parameters.AddWithValue("p",password.text);
        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();
    }
}
