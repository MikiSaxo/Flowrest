using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnAnimButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> _buttonsList;
    [SerializeField] private float _timeBetweenEachButtons;

    private float _timeWaitToSpawnAnim;
    
    public void LaunchSpawnAnim()
    {
        _timeWaitToSpawnAnim = 0;
        StartCoroutine(SpawnAnim());
    }

    public void LaunchSpawnAnimDelay()
    {
        _timeWaitToSpawnAnim = 1;
        StartCoroutine(SpawnAnim());
    }

    IEnumerator SpawnAnim()
    {
        foreach (var but in _buttonsList)
        {
            but.transform.DOScale(0, 0);
        }
        
        yield return new WaitForSeconds(_timeWaitToSpawnAnim);
        

        foreach (var but in _buttonsList)
        {
            yield return new WaitForSeconds(_timeBetweenEachButtons);
            but.transform.DOScale(1, 0);
            but.GetComponent<PointerMotion>().Bounce();
        }
    }

    public void AddToButtonList(GameObject newObj)
    {
        _buttonsList.Add(newObj);
    }

    public void ClearButtonList()
    {
        _buttonsList.Clear();
    }
}
