using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GroundSceneManager : MonoBehaviour
{
    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "TempSamScene":
                if(GetComponent<WaterSourceManager>()) GetComponent<WaterSourceManager>().enabled = true;
                else GetComponent<GroundMainManager>().enabled = true;
                if(GetComponent<WaterFlowing>()) GetComponent<WaterSourceManager>().enabled = true;
                GetComponent<GroundEditorManager>().enabled = false;
                print("tempscene");
                break;
            case "LevelEditor":
                if(GetComponent<WaterSourceManager>()) GetComponent<WaterSourceManager>().enabled = false;
                else GetComponent<GroundMainManager>().enabled = false;
                if(GetComponent<WaterFlowing>()) GetComponent<WaterFlowing>().enabled = false;
                GetComponent<GroundEditorManager>().enabled = true;
                print("editorscene");
                break;
        }
    }
}