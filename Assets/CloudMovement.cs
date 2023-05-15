using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CloudMovement : MonoBehaviour
{
    public void InitMovement(Transform endPos, float timeToEnd)
    {
        // J'utilise dotween pour modifier SEULEMENT la position en X avec le "DOMoveX"
        // Je lui donne le comportement Ease.Linear car je veux qu'il se déplace de manière linéaire
        // Quand il a fini de ce déplacer, avec le "OnComplete", j'appelle la fonction "DestroyCloud" pour le détruire 
        gameObject.transform.DOMoveX(endPos.position.x, timeToEnd).SetEase(Ease.Linear).OnComplete(DestroyCloud);
    }

    private void DestroyCloud()
    {
        // Je détruis le nuage 
        Destroy(gameObject);
    }
}
