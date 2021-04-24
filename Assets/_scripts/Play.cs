using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    public float speed = 0.025f;
    private Animator AnimaObj;
    // Start is called before the first frame update
    private Transform ballTran;
    public Transform playerTs;
    public float colliderRadius;    //碰撞半径
    public float force=1.0f;
    public float shootPower = 15.0f;
    public float passForce = 30.0f;
    private actUtils utils;
    private PlayControl pc;

    public actUtils.Team team;

    

    void Start()
    {
        AnimaObj = this.GetComponent<Animator>();
        ballTran = GameObject.FindGameObjectWithTag("Ball").transform;
        utils = new actUtils(this.transform,ballTran,AnimaObj,speed,colliderRadius,force,shootPower,passForce,team);
        pc= new PlayControl(utils);
    }

    // Update is called once per frame
    void Update()
    {
        pc.Play();
        utils.DribbleMove();  
        this.GetComponent<aiMove>().enabled = !this.GetComponent<Play>().enabled;
    }

    void shootForce(){
        utils.shootForce();
    }
}
