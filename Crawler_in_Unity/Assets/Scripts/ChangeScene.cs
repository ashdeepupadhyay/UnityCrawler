using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene :MonoBehaviour{
    public void ChangeMenuScene(string scenename)
    {     
        SceneManager.LoadScene(scenename, LoadSceneMode.Single);
    }
}	
