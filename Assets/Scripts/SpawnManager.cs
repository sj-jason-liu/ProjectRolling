using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //SpawnManager will generate people randomly on a spot to passby
    //the shooting scene.
    //It should be at least 3 start positions and 3 dest positions to make it randomize.

    //then create a random chance to make generated person walk toward shooting scene.

    [SerializeField]
    private Transform[] _startPosition, _endPosition;

    [SerializeField]
    private GameObject _peoplePrefab;

    private bool _isSpawning;

    void Start()
    {
        _isSpawning = true;
        StartCoroutine(GeneratePeople());
    }

    void Update()
    {
        
    }

    IEnumerator GeneratePeople()
    {
        while(true)
        {
            while(_isSpawning)
            {
                int startIndex = Random.Range(0, 3);
                int endIndex = Random.Range(0, 3);
                GameObject spawnedPerson = Instantiate(_peoplePrefab, _startPosition[startIndex].position, transform.rotation);
                spawnedPerson.GetComponent<Somebody>().DestinationTarget(_endPosition[endIndex]);
                yield return new WaitForSeconds(Random.Range(2f, 7f));
            }
            break;
        }
    }
}
