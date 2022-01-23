using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console
            Debug.Log("Do something here");

            SceneManager.LoadScene(sceneName: "Track2");
        }
    }
   
}

