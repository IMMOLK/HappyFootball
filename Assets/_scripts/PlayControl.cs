using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControl : MonoBehaviour
{
   private KeyCode[] control;
   private actUtils utils;
   private Animator AnimaObj;
   public PlayControl(actUtils utils){
       this.utils = utils;
       AnimaObj = utils.GetAnimator();
       control = utils.GetKeyCodes();
   }

    public void Play(){

        if(Input.GetKey(control[4])){
            AnimaObj.SetBool("run",true);
            utils.run();
            return;
        }

        if (Input.GetKey(control[0]))
        {
            utils.walk(Vector3.right);
        }
        else if (Input.GetKey(control[1]))
        {
            utils.walk(Vector3.left);
        }
        
        if (Input.GetKey(control[2]))
        {
            utils.walk(Vector3.forward);
        }else if (Input.GetKey(control[3]))
        {
            utils.walk(Vector3.back);
        }   

        if(Input.GetKeyUp(control[0])||Input.GetKeyUp(control[1])||Input.GetKeyUp(control[2])||Input.GetKeyUp(control[3])){
            utils.resetAnim();
        }
        
        if(Input.GetKeyUp(control[4])){
            AnimaObj.SetBool("run",false);
        }

        if (Input.GetKey(control[5]))
        {
            utils.Pass(true);
        }

        if(Input.GetKey(control[6])){
            if(utils.GetTeam() == actUtils.Team.Blue){
                utils.quickSubs("Blue");
            }else{
                utils.quickSubs("Red");
            }
            
        }

        if (Input.GetKey(control[7]))
        {
            utils.Shoot();
        }


    }
}
