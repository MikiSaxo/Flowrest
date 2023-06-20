using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnAnimButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> _buttonsList;
    [SerializeField] private float _timeBetweenEachButtons;
    [SerializeField] private float _timeSpawnDelay = 1f;

    private float _timeWaitToSpawnAnim;
    
    public void LaunchSpawnAnim()
    {
        _timeWaitToSpawnAnim = 0;
        StartCoroutine(SpawnAnim());
    }

    public void LaunchSpawnAnimDelay()
    {
        _timeWaitToSpawnAnim = _timeSpawnDelay;
        foreach (var but in _buttonsList)
        {
            but.transform.DOScale(0, 0);
        }
        
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

    public void ResetScaleButtons()
    {
        foreach (var but in _buttonsList)
        {
            but.transform.DOScale(0, 0);
        }
    }

    public void UpdateCanEnter(bool state)
    {
        foreach (var but in _buttonsList)
        {
            but.GetComponent<PointerMotion>().UpdateCanEnter(state);
        }
    }
    public void AddToButtonList(GameObject newObj)
    {
        _buttonsList.Add(newObj);
    }

    public void RemoveButtonToList(int index)
    {
        _buttonsList.RemoveAt(index);
    }

    public void ClearButtonList()
    {
        _buttonsList.Clear();
    }
}
