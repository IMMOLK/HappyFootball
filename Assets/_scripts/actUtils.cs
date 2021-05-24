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
    public float goalDir = 17.0f;
    public enum Team{
        None = 0,
        Blue = 1,
        Red = 2
    }

    public actUtils(){}

    public actUtils(Transform bal){ballTran = bal;}
    private KeyCode[] BlueControl = {KeyCode.D,KeyCode.A,KeyCode.W,KeyCode.S,KeyCode.LeftShift,KeyCode.J,KeyCode.K,KeyCode.Space};
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

    private Vector3 direction;
    public Vector3 getDirection(){
        direction = ballTran.position - player.position;
        direction.y=0;
        return direction;
    }

    public void PlayerRotate(Vector3 v){
        player.rotation = Quaternion.Slerp(player.rotation, Quaternion.LookRotation(v), 10.0f * Time.deltaTime);
    }

    public void walk(Vector3 v){
        AnimaObj.SetBool("forward",true);
        PlayerRotate(v); 
        player.Translate(v * speed*Time.deltaTime,Space.World);
    }

    public void DribbleMove(){
        if(isHoldingBall()){
            AnimaObj.SetBool("dribble",true);
            Debug.Log("isHoldingBall");
            ballTran.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(getDirection())*force);
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
        player.tag = newPlayer.tag;
        newPlayer.tag = temp;
    }

    public void quickSubs(string tag){
        Transform subPlayer = queryClosest(tag,ballTran.position);
        Substitution(subPlayer.transform);
    }

    public Transform queryClosest(string tag,Vector3 v){
        GameObject[] players = GameObject.FindGameObjectsWithTag(tag);
        Transform closestPlayer = player;
        if(player==null || v == player.position)
            closestPlayer = null;
        foreach(GameObject p in players){
            Vector3 lens1 = v - p.transform.position;
            if(closestPlayer == null){
                closestPlayer = p.transform;
            }
            Vector3 lens2 = v - closestPlayer.position;
            if(Vector3.Magnitude(lens1) < Vector3.Magnitude(lens2)){
                closestPlayer = p.transform;
            }
        }
        return closestPlayer;
    }

    string searchTag;
    public void Pass(bool isSubs){
        if(!isHoldingBall()){
            return;
        }
        switch(team){
            case Team.Blue: searchTag = "Blue";
                            break;
            case Team.Red:  searchTag = "Red";
                            break;
        }
        Transform teammateTran;
        if(player.tag == "BluePlayer"){
            teammateTran = queryClosest(searchTag,player.position);
        }else{
            teammateTran = queryClosest(searchTag,getGoalPos());
        }
        if(teammateTran == player){
            Shoot();
            return;
        }
        AnimaObj.Play("pass",0,0f);
        Vector3 direction = teammateTran.position-ballTran.position;
        direction.y = 0;
        ballTran.GetComponent<Rigidbody>().AddForce(direction*passForce);
        if(isSubs){
            Substitution(teammateTran);
        }
        teammateTran = null;
    }



    public void run(){
        AnimaObj.SetBool("run",true);
        PlayerRotate(getDirection());
        player.Translate(Vector3.forward * speed*Time.deltaTime);
    }



    //Ai 行为函数
    private float gd;
    private Vector3 getGoalPos(){
        gd = goalDir;
        if(GameControl.isChangeGoal)
            gd = -gd;
        if(team==actUtils.Team.Blue){
            return new Vector3(gd,0,0);
        }else if(team==actUtils.Team.Red){
            return new Vector3(-gd,0,0);
        }else{
            Debug.Log("getGoalDir() error");
            return new Vector3(999,999,999);
        }
    }

    public void Dribbling(){
        // Debug.Log("Dribble"+Vector3.Dot(getGoalPos(), player.forward));
        if (Vector3.Dot(getGoalPos(), player.forward) < 0)
            {
                setState(AiControl.PlayerState.Pass);
                return;
            }
        // Debug.Log("Dribbling");
        walk(Vector3.Normalize(getGoalPos()-player.position));
    }
    public void ReturnDefense()
    {
        if (team==Team.Blue)
        {
            walk(Vector3.left);
        }
        else
        {
            walk(Vector3.right);
        }
 
    }

    public void KickForward()
    {
        // Debug.Log("Shoot"+Vector3.Dot(getGoalPos(), player.forward));
        if (Vector3.Dot(getGoalPos(), player.forward) > 0)
        {
            Shoot();
        }else{
            setState(AiControl.PlayerState.Pass);
        }
    }

    public void Chase()
    {   
        // Debug.Log("chase");
        run();
    }

    private bool flag = true;
    public void resetFlag(){
        flag = true;
    }
    public void RunPosition()
    {
        float a = 0;
        if(flag){
            a = getRandom();
            flag = false;
        }
        // Debug.Log("RunPosition");
        if(player == queryClosest(player.tag,-getGoalPos())){
            if(System.Math.Abs(player.position.x)<System.Math.Abs(ballTran.position.x)){
                ReturnDefense();
            }
        }else if(player == queryClosest(player.tag,ballTran.position)){
            Chase();
        }else{
            resetAnim();
            if(a>0.5){
                AnimaObj.SetBool("right",true);
                player.Translate(player.right * speed*Time.deltaTime,Space.Self);
            }else{
                AnimaObj.SetBool("left",true);
                player.Translate(-player.right * speed*Time.deltaTime,Space.Self);
            }
            
        }
        
    }

    public float getRandom(){
        return Random.Range(0f, 1f);
        
    }

    public void Steals()
    {
        //抢断：用来干扰对方进攻，
        //如果球在范围内，并且如果当前面向地方大门，则踢一脚
                     
    }

    public void WaitPass()
    {
        // Debug.Log("WaitPass");
        //指令 原地等待
        //注视球飞过来 (后期再解决旋转问题，目前先不写)
        
    }

    public void setState(AiControl.PlayerState state){
        player.GetComponent<Play>().playerState = state;
    }




    //其它脚本从此脚本获取变量或属性
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



    private string[] animators = {"forward","backward","left","right","run"};
    public void resetAnim(){
        foreach(string str in animators){
            AnimaObj.SetBool(str,false);
        }
    }

}
