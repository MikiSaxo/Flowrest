using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private bool _isFullFloor;
    private AllStates _fullFloorState;
    private bool _isFullFloorCompleted;

    
    public void InitQuestFullFloor(AllStates whichState)
    {
        _isFullFloor = true;
        _fullFloorState = whichState;
    }

    public void CheckQuest()
    {
        if(_isFullFloor)
            print("full floor completed? : " + CheckFullFloorQuest());
    }

    private bool CheckFullFloorQuest()
    {
        GameObject[,] map = MapManager.Instance.MapGrid;
        _isFullFloorCompleted = true;
        
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if(map[x,y] == null) continue;
                
                if (map[x, y].GetComponent<GroundStateManager>() == null)
                    continue;
                
                if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() == AllStates.None)
                    continue;
                
                if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() != _fullFloorState)
                    return false;
            }
        }

        return true;
    }
}
