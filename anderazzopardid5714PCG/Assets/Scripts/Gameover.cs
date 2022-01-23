using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Gameover : MonoBehaviour
{
    // Start is called before the first frame update
   public void RestartButton()
    {
        SceneManager.LoadScene("Maze");
    }
}
