using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiControl : MonoBehaviour
{
    float coolingTimer = 2.0f;            //冷却计时
    float cooling = 1.0f;                  //冷却;

    private actUtils utils;
    public enum PlayerState
    {
        //进攻
        Shoot = 0,                 //射门
        Dribbling = 1,             //带球           
        Pass = 2,                  //传球
        WaitPass = 3,              //待传
        //防守
        Steals = 10,               //抢断
        ReturnDefense = 11,        //回防
        //通用
        Chase = 20,               //追球
        RunPosition = 21,         //跑位
        KickForward, //向前踢
        //等待守门员开球
        Static = 30,              //静止              不写
        //开局开球              
        WaitInPosition = 40         //在固定位置等待      不写    
    };
    
    public AiControl(actUtils utils){
        this.utils = utils;
    }
    public void PersistenceState(PlayerState ps)
    {
        switch (ps)
        {
            case PlayerState.Shoot:
                utils.KickForward();
                break;
            case PlayerState.Chase:
                utils.Chase();
                break;
            case PlayerState.Pass:
                utils.Pass(false);
                break;
            case PlayerState.RunPosition:
                utils.RunPosition();
                break;
            case PlayerState.WaitPass:
                utils.WaitPass();
                break;
            case PlayerState.Dribbling:
                utils.Dribbling();
                break;
             case PlayerState.ReturnDefense:
                utils.ReturnDefense();
                break;
        }
    }


    public void ChangeState()
    {
        if(coolingTimer>0){
            coolingTimer -= Time.deltaTime;
            return;
        }
        if(utils.isHoldingBall()){
            float a = utils.getRandom();
            if(a>=0.8){
                utils.setState(PlayerState.Dribbling);
            }
            else if (a>=0.3)
            {
                utils.setState(PlayerState.Shoot);
            }
            else
            {
                utils.setState(PlayerState.Pass);
            }
        }else{
            float b = utils.getRandom();
            if (b>=0.6)
            {
                utils.setState(PlayerState.Chase);
            }
            else
            {
                utils.setState(PlayerState.RunPosition);
            }
        }
        coolingTimer = cooling;
        utils.resetFlag();
    }

    

}
