using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class clockButton : MonoBehaviour
{
    public string nameScene;
    public void ClickChange(){
        SceneManager.LoadScene(nameScene);
    }
}
