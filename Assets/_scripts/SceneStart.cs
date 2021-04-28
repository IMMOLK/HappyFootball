using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStart : MonoBehaviour
{
    public void onStart(int sceneNumber){
        SceneManager.LoadScene(sceneNumber);
    }

    public void setPersonNumber(int pn){
        PlayerPrefs.SetInt("PerNum",pn);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
