using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloudManager : MonoBehaviour
{
    [SerializeField] private Transform[] _startPos;
    [SerializeField] private Transform _endPos;
    [SerializeField] private GameObject _cloudPrefab;
    [SerializeField] private float _timeToSpawn;
    [SerializeField] private float _timeToEnd;
    private float _cooldown;

    private void Start()
    {
    }

    private void SpawnCloud()
    {
        // Je choisi un nombre aléatoire entre 0 et le nombre de position de départ de nuage
        int randomStartPos = Random.Range(0, _startPos.Length);
        // Je créé un nuage à la position de l'index défini par le randomStartPos qui vient du tableau de StartPos
        GameObject go = Instantiate(_cloudPrefab, _startPos[randomStartPos].transform);
        // Je récupère le Script "CloudMovement" du nuage que je viens de créer pour lui envoyer la position finale et le temps qu'il faut pour y arriver
        go.GetComponent<CloudMovement>().InitMovement(_endPos, _timeToEnd);
    }

    private void Update()
    {
        // Je réduit le temps du cooldown
        _cooldown -= Time.deltaTime;
         
        // Si ce cooldown est inférieur ou égal à 0 alors 
        if(_cooldown <= 0)
        {
            // Je crée un nuage en appelant la fonction "SpawnCloud"
            SpawnCloud();
            // Je reset le cooldown
            _cooldown = _timeToSpawn;
        }
    }
}
