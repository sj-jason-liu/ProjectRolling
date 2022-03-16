using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 3f;
    private float _horizontalInput;
    private float _verticalInput;
    private float _yVelocity;
    [SerializeField]
    private float _pickUpRate = 0.5f;
    private float _canPickUp = -1f;

    private CharacterController _controller;
    [SerializeField]
    private GameObject _itemHolder;

    [SerializeField] private bool _isHolding;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Moving();
        transform.position =
            new Vector3(Mathf.Clamp(transform.position.x, -9f, 9f), transform.position.y, Mathf.Clamp(transform.position.z, -9f, 6f));
    }

    void Moving()
    {
        Vector3 position = transform.position;

        //get the horizontal and vertical input
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        //set input value as a new Vector3
        Vector3 move = new Vector3(_horizontalInput, 0f, _verticalInput);

        if (!_controller.isGrounded)
        {
            _yVelocity -= 1f;
        }

        //set the moving direction as facing target
        //rotating y-axis to make vector3.forward facing moving direciton
        if (move.x != 0 || move.z != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _speed);
        }

        //Limit the moveable area within a range
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, -12f, 12f), transform.position.y, Mathf.Clamp(transform.position.z, -9.5f, 5f));

        //if left shift pressed
        //double the speed
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed *= 2f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed /= 2f;
        }

        _controller.Move(move * _speed * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Item")
        {
            if(Input.GetKey(KeyCode.E) && Time.time > _canPickUp)
            {
                if(!_isHolding)
                {
                    other.gameObject.transform.parent = _itemHolder.transform;
                    other.transform.position = _itemHolder.transform.position;
                    //other.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    other.GetComponent<Rigidbody>().isKinematic = true;
                    _isHolding = true;
                }
                else
                {
                    other.gameObject.transform.parent = null;
                    //other.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    other.GetComponent<Rigidbody>().isKinematic = false;
                    _isHolding = false;
                }
                _canPickUp = Time.time + _pickUpRate;
            }
        }
    }
}
