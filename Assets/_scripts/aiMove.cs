using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiMove : MonoBehaviour
{
    private GameObject ball;
    private Animator AnimaObj;
    public Vector3 speed;
    public Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        AnimaObj = this.GetComponent<Animator>();
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    // Update is called once per frame
    void Update()
    {
        AnimaObj.SetBool("forward",true);
        direction = ball.transform.position - this.transform.position;
        direction.y = 0;
        speed = 1.0f * direction/Vector3.Magnitude(direction);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 1.0f * Time.deltaTime);
        this.transform.position += speed * Time.deltaTime;
    }
}
