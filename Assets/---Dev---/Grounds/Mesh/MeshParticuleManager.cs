using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshParticuleManager : MonoBehaviour
{
    // Start is called before the first frame update
    // [SerializeField] private GameObject _particulePrefab;
    [SerializeField] private GameObject _particuleParent;
    [SerializeField] private float _timeSpawnMin;
    [SerializeField] private float _timeSpawnMax;
    private float _cooldown;

    private void Start()
    {
        float randomCooldown = Random.Range(_timeSpawnMin, _timeSpawnMax);
        _cooldown = randomCooldown;
    }

    private void SpawnParticule()
    {
        var getMeshManager = GetComponent<MeshManager>();
        
        if(getMeshManager == null) return;
        
        var getParticule = getMeshManager.GetSpecificParticule();
        
        if(getParticule == null) return;
        
        Instantiate(getParticule, _particuleParent.transform);
        float randomCooldown = Random.Range(_timeSpawnMin, _timeSpawnMax);
        _cooldown = randomCooldown;
    }

    private void Update()
    {
        // Je reduit le temps du cooldown
        _cooldown -= Time.deltaTime;

        // Si ce cooldown est inferieur ou egal a 0 alors 
        if (_cooldown <= 0)
        {
            SpawnParticule();
        }
    }
}
