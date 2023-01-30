using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlainMesh : MonoBehaviour
{
    [SerializeField] private GameObject[] _trees;
    [SerializeField] private float _timeToSpawn;
    private int count;

    public void EnterState()
    {
        foreach (var tree in _trees)
        {
            tree.transform.DOScale(Vector3.zero, 0);
            // tree.transform.DOScale(Vector3.one, _timeToSpawn);
        }
        Sequence mySequence = DOTween.Sequence();
        // Add a movement tween at the beginning
        mySequence.Append(_trees[0].transform.DOScale(Vector3.one, _timeToSpawn));
        // Add a rotation tween as soon as the previous one is finished
        mySequence.Append(_trees[1].transform.DOScale(Vector3.one, _timeToSpawn));
        mySequence.Append(_trees[2].transform.DOScale(Vector3.one, _timeToSpawn));
        // Delay the whole Sequence by 1 second
        // mySequence.PrependInterval(1);
        // Insert a scale tween for the whole duration of the Sequence
        // mySequence.Insert(0, transform.DOScale(new Vector3(3,3,3), mySequence.Duration()));
    }
}