using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actUtils : MonoBehaviour
{
   
    private Transform player;
    private Transform ballTran;
    private Animator AnimaObj;
    private float speed;
    private float colliderRadius;
    private float force;
    private float shootPower;
    private float passForce;
    private Team team;
    public enum Team{
        Blue = 0,
        Red = 1
    }

    void Start() {

    }
    public actUtils(){}

    public actUtils(Transform bal){ballTran = bal;}
    private KeyCode[] BlueControl = {KeyCode.D,KeyCode.A,KeyCode.W,KeyCode.S,KeyCode.LeftControl,KeyCode.R,KeyCode.LeftShift,KeyCode.Space};
    private KeyCode[] RedControl = {KeyCode.RightArrow,KeyCode.LeftArrow,KeyCode.UpArrow,KeyCode.DownArrow,KeyCode.Keypad0,KeyCode.Keypad1,KeyCode.Keypad2,KeyCode.Keypad3};
    public actUtils(Transform pla,Transform bal,Animator ani,float spe,float col,float forc,float sho,float pas,Team tea){
        this.player = pla;
        this.ballTran = bal;
        this.AnimaObj = ani;
        this.speed = spe;
        this.colliderRadius = col;
        this.force = forc;
        this.shootPower = sho;
        this.passForce = pas;
        this.team = tea;
    }

    public bool isHoldingBall(){
        return getDirection().magnitude<colliderRadius;
    }

    public Vector3 getDirection(){
        return (ballTran.position - player.position);
    }

    public void PlayerRotate(Vector3 v){
        player.rotation = Quaternion.Slerp(player.rotation, Quaternion.LookRotation(v), 10.0f * Time.deltaTime);
    }

    public void walk(Vector3 v){
        AnimaObj.SetBool("forward",true);
        PlayerRotate(v); 
        player.Translate(v * speed,Space.World);
    }

    public void DribbleMove(){
        if(isHoldingBall()){
            AnimaObj.SetBool("dribble",true);
            Debug.Log("isHoldingBall");
            ballTran.GetComponent<Rigidbody>().AddForce(getDirection()/Vector3.Magnitude(getDirection())*force);
        }else{
            AnimaObj.SetBool("dribble",false);
        }
    }

    public void Shoot(){
        if(isHoldingBall()){
            AnimaObj.Play("shoot",0,0f);
            player.GetComponent<Play>().Invoke("shootForce",0.5f);
        }else{
           
        }
    }

    public void shootForce(){
        ballTran.GetComponent<Rigidbody>().AddExplosionForce(shootPower,player.position,3.0f,0.1f);
    }

    

    public void Substitution(Transform newPlayer){
        string temp = player.tag;
        player.GetComponent<Play>().enabled = false;
        player.tag = newPlayer.tag;
        newPlayer.GetComponent<Play>().enabled = true;
        newPlayer.tag = temp;
    }

    public void quickSubs(string tag){
        Transform subPlayer = queryClosest(tag);
        Substitution(subPlayer.transform);
    }

    public Transform queryClosest(string tag){
        GameObject[] players = GameObject.FindGameObjectsWithTag(tag);
        Transform closestPlayer = player;
        foreach(GameObject p in players){
            Vector3 lens1 = ballTran.position - p.transform.position;
            if(closestPlayer == null){
                closestPlayer = p.transform;
            }
            Vector3 lens2 = ballTran.position - closestPlayer.position;
            if(Vector3.Magnitude(lens1) < Vector3.Magnitude(lens2)){
                closestPlayer = p.transform;
            }
        }
        return closestPlayer;
    }

    private Transform teammateTran;
    public void Pass(){
        if(!isHoldingBall()){
            Debug.Log("没有持球！");
            return;
        }
        Collider[] allColls = Physics.OverlapSphere(player.position,10.0f);
        foreach(Collider item in allColls){
            if (team==Team.Blue&& item.tag == "Blue")
            {
                teammateTran = item.transform;
                break;      //这一步表示，不论有多少个队友，只查找集合的第一个，并返回
            }
            if (team==Team.Red&&item.tag=="Red")
            {
                teammateTran = item.transform;
                break;
            }
        }
        if(teammateTran == null){
                Debug.Log("未找到队友");
                return;
        }
        AnimaObj.Play("pass",0,0f);
        Vector3 direction = teammateTran.position-ballTran.position;
        ballTran.GetComponent<Rigidbody>().AddForce(direction/Vector3.Magnitude(direction)*passForce);
        Substitution(teammateTran);
        teammateTran = null;
    }


    public void run(){
        Vector3 direction = getDirection();
        direction.y = 0;
        PlayerRotate(direction);
        //player.Translate(Vector3.forward * speed);
    }





    public Team GetTeam(){
        return team;
    }

    public Animator GetAnimator(){
        return AnimaObj;
    }


    public KeyCode[] GetKeyCodes(){
        if(team == Team.Blue){
            return BlueControl;
        }
        return RedControl;
    }

}
