using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class oGroundSceneManager : MonoBehaviour
{
    private void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "TempSamScene":
                if(GetComponent<WaterSourceManager>()) GetComponent<WaterSourceManager>().enabled = true;
                else GetComponent<oGroundMainManager>().enabled = true;
                if(GetComponent<WaterFlowing>()) GetComponent<WaterFlowing>().enabled = true;
                if(GetComponent<oWaterEditorManager>()) GetComponent<oWaterEditorManager>().enabled = false;
                GetComponent<oGroundEditorManager>().enabled = false;
                // print("tempscene");
                break;
            case "LevelEditor":
                if(GetComponent<WaterSourceManager>()) GetComponent<WaterSourceManager>().enabled = false;
                else GetComponent<oGroundMainManager>().enabled = false;
                if(GetComponent<WaterFlowing>()) GetComponent<WaterFlowing>().enabled = false;
                if(GetComponent<oWaterEditorManager>()) GetComponent<oWaterEditorManager>().enabled = true;
                GetComponent<oGroundEditorManager>().enabled = true;
                // print("editorscene");
                break;
        }
    }
}