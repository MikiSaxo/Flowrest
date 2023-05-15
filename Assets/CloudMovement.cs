using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CloudMovement : MonoBehaviour
{
    public void InitMovement(Transform endPos, float timeToEnd)
    {
        // J'utilise dotween pour modifier SEULEMENT la position en X avec le "DOMoveX"
        // Je lui donne le comportement Ease.Linear car je veux qu'il se d�place de mani�re lin�aire
        // Quand il a fini de ce d�placer, avec le "OnComplete", j'appelle la fonction "DestroyCloud" pour le d�truire 
        gameObject.transform.DOMoveX(endPos.position.x, timeToEnd).SetEase(Ease.Linear).OnComplete(DestroyCloud);
    }

    private void DestroyCloud()
    {
        // Je d�truis le nuage 
        Destroy(gameObject);
    }
}
