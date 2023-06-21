using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharaMovement : MonoBehaviour
{
    [SerializeField] private Image _imgChara;
    [SerializeField] private Transform[] _tpPoints;
    [SerializeField] private float _timeMove;

    private void LaunchMovement()
    {
        gameObject.transform.DOMoveX(_tpPoints[0].position.x, 0);
        gameObject.GetComponent<Image>().DOFade(0, 0);

        gameObject.transform.DOMoveX(_tpPoints[1].position.x, _timeMove);
        gameObject.GetComponent<Image>().DOFade(1, _timeMove);
    }

    public void UpdateChara(Sprite sprite)
    {
        if (sprite == null)
        {
            _imgChara.enabled = false;
            return;
        }

        _imgChara.enabled = true;
        _imgChara.sprite = sprite;

        LaunchMovement();
    }
}