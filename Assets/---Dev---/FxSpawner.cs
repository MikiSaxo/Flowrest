using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FxSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _startPos;
    [SerializeField] private Transform _endPos;
    [SerializeField] private GameObject[] _fxPrefab;
    [SerializeField] private float _timeToSpawn;
    [SerializeField] private float _timeToEnd;
    private float _cooldown;

    private void Start()
    {
    }

    private void SpawnFx()
    {
        // Je choisi un nombre aléatoire entre 0 et le nombre de position de départ de nuage
        int randomStartPos = Random.Range(0, _startPos.Length);
        int randomFx = Random.Range(0, _fxPrefab.Length);
        // Je créé un nuage à la position de l'index défini par le randomStartPos qui vient du tableau de StartPos
        GameObject go = Instantiate(_fxPrefab[randomFx], _startPos[randomStartPos].transform);

        // Je récupère le Script "CloudMovement" du nuage que je viens de créer pour lui envoyer la position finale et le temps qu'il faut pour y arriver
        if (go.GetComponent<CloudMovement>() != null)
        {
            go.GetComponent<CloudMovement>().InitMovement(_endPos, _timeToEnd);
        }
    }

    private void Update()
    {
        // Je réduit le temps du cooldown
        _cooldown -= Time.deltaTime;

        // Si ce cooldown est inférieur ou égal à 0 alors 
        if (_cooldown <= 0)
        {
            // Je crée un nuage en appelant la fonction "SpawnCloud"
            SpawnFx();
            // Je reset le cooldown
            _cooldown = _timeToSpawn;
        }
    }
}
