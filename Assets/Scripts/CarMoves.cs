using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarMoves : MonoBehaviour
{
    private bool _hasArrivedSiteB;

    [SerializeField]
    private Transform _siteA, _siteB;

    private NavMeshAgent _navMeshAgent;
    
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if(_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is NULL!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, _siteB.position) < 0.3f)
        {
            if(!_hasArrivedSiteB)
            {
                StartCoroutine(ResetPositionRoutine());
            }
        }
        else
        {
            _navMeshAgent.SetDestination(_siteB.position);
        }
        //if car is not on site b
        //moves to site b
        //wait for 5-8 secs
        //set the position back to site a
        //flip the switch
    }

    IEnumerator ResetPositionRoutine()
    {
        _hasArrivedSiteB = true;
        yield return new WaitForSeconds(Random.Range(5f, 8f));
        transform.position = _siteA.position;
        _hasArrivedSiteB = false;
    }
}
