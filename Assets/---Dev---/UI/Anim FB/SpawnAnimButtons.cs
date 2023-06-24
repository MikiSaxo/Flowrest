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
        _timeWaitToSpawnAnim = 0.05f;
        ResetScaleButtons();

        StartCoroutine(SpawnAnimButton());
    }

    public void LaunchSpawnAnimDelay()
    {
        _timeWaitToSpawnAnim = _timeSpawnDelay;
        ResetScaleButtons();
        
        StartCoroutine(SpawnAnimButton());
    }

    IEnumerator SpawnAnimButton()
    {
        yield return new WaitForSeconds(_timeWaitToSpawnAnim);
        
        foreach (var but in _buttonsList)
        {
            yield return new WaitForSeconds(_timeBetweenEachButtons);
            but.transform.DOScale(1, 0);
            but.GetComponent<PointerMotion>().Bounce();
        }
    }

    private void SpawnButton()
    {
        foreach (var but in _buttonsList)
        {
            // yield return new WaitForSeconds(_timeBetweenEachButtons);
            but.transform.DOScale(1, 0);
            but.GetComponent<PointerMotion>().Bounce();
        }
    }

    public void ResetScaleButtons()
    {
        // StopCoroutine(SpawnAnim());

        foreach (var but in _buttonsList)
        {
            but.transform.DOKill();
            but.transform.DOScale(0, 0);
            but.transform.DOComplete();
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
