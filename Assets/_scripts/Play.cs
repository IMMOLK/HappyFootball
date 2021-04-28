using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    public float speed = 3f;
    private Animator AnimaObj;
    // Start is called before the first frame update
    private Transform ballTran;
    public float colliderRadius;    //碰撞半径
    public float force=1.0f;
    public float shootPower = 7.0f;
    public float passForce = 10.0f;
    private actUtils utils;
    private PlayControl pc;
    private AiControl aiControl;
    public AiControl.PlayerState playerState;


    public actUtils.Team team;

    

    void Start()
    {
        AnimaObj = this.GetComponent<Animator>();
        ballTran = GameObject.FindGameObjectWithTag("Ball").transform;
        utils = new actUtils(this.transform,ballTran,AnimaObj,speed,colliderRadius,force,shootPower,passForce,team);
        pc= new PlayControl(utils);
        aiControl = new AiControl(utils);
    }

    // Update is called once per frame
    void Update()
    {
        if(tag=="BluePlayer"||tag=="RedPlayer"){
            pc.Play();
        }else{
            AnimaObj.SetBool("forward",true);
            aiControl.ChangeState();
            aiControl.PersistenceState(playerState);
        }
        utils.DribbleMove();  
    }

    void shootForce(){
        utils.shootForce();
    }
}
