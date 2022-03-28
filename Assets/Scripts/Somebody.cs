using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Somebody : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;

    [SerializeField]
    private float _detectRange = 3f;

    private Transform _target;

    private Vector3 _originPosition;

    private CharacterController _controller;

    private bool _isSomebodyNoticed;
    
    //somebody should curious about shooting scene
    //or walkby close to the shooting scene
    //when player walk toward them
    //they will walk away immediately

    //some moving patterns
    //A: try to get close to shooting scene
    //B: Walkby shooting scene
    
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if(_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent is NULL!");
        }
        _originPosition = transform.position;
        Debug.Log("Somebody is ready!");
    }

    // Update is called once per frame
    void Update()
    {   
        switch(_isSomebodyNoticed)
        {
            case true:
                _navMeshAgent.destination = _originPosition;
                
                break;
            case false:
                _navMeshAgent.destination = _target.position;
                break;
        }
        if(Vector3.Distance(transform.position, _target.position) < 0.5f)
        {
            Destroy(gameObject, 3f);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.tag == "Player")
    //    {
    //        _isSomebodyNoticed = true;
    //    }
    //}

    public void DestinationTarget(Transform targetPosition)
    {
        _target = targetPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectRange);
    }
}
