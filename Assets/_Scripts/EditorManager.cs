using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEditorInternal;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    [SerializeField] private Transform _level;
    [SerializeField] private GameObject[] _environment;
    [SerializeField] private Vector2Int _mapSize;

    private GameObject[,] MapGrid;
    private List<GameObject> _tempGrid = new List<GameObject>();


    void Start()
    {
        InitializeLevel(_mapSize);
    }

    private void InitializeLevel(Vector2Int sizeMap) //Map creation
    {
        MapGrid = new GameObject[_mapSize.x, _mapSize.y];
        for (int i = 0; i < sizeMap.x; i++)
        {
            for (int j = 0; j < sizeMap.y; j++)
            {
                GameObject go = Instantiate(_environment[0], _level.transform);
                InitObj(go, i, j);
            }
        }
    }

    private void InitObj(GameObject which, int i, int j)
    {
        // Tp ground to its position
        which.transform.position = new Vector3(-j, 0, i);
        _tempGrid.Add(which);
        // Change coords of the ground
        //if (hasGroundManager)
        //  which.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, _mapSize.x - 1 - i));
        // Update _mapGrid
        //MapGrid[j, _mapSize.x - 1 - i] = which;
        // Spawn anim
    }

    private void UpdateMap()
    {
        foreach (var temp in _tempGrid)
        {
            Destroy(temp);
        }

        _tempGrid.Clear();
        InitializeLevel(_mapSize);
    }

    public void ChangeWidth(int which)
    {
        if (_mapSize.x - Mathf.Abs(which) <= 0 && which < 0)
            return;
        _mapSize.x += which;
        UpdateMap();
    }

    public void ChangeHeight(int which)
    {
        if (_mapSize.y - Mathf.Abs(which) <= 0 && which < 0)
            return;
        _mapSize.y += which;
        UpdateMap();
    }
}