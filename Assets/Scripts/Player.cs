using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 3f;
    private float _horizontalInput;
    private float _verticalInput;
    private float _yVelocity;
    [SerializeField]
    private float _gravity = 9.81f;
    [SerializeField]
    private float _pickUpRate = 0.5f;
    private float _canPickUp = -1f;

    private CharacterController _controller;
    [SerializeField]
    private GameObject _itemHolder;

    [SerializeField] private bool _isHolding;

    //input actions asset (Assets/Input/PlayerControls.inputactions), action map "Player"
    [SerializeField]
    private InputActionAsset _inputActions;
    private InputActionMap _playerActionMap;
    private InputAction _moveAction;
    private InputAction _sprintAction;
    private InputAction _interactAction;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null)
        {
            Debug.LogError("CharacterController is NULL!");
        }

        if (_inputActions == null)
        {
            Debug.LogError("Input Actions is NULL!");
            return;
        }

        //find the actions in the "Player" map and enable it
        _playerActionMap = _inputActions.FindActionMap("Player", true);
        _moveAction = _playerActionMap.FindAction("Move", true);
        _sprintAction = _playerActionMap.FindAction("Sprint", true);
        _interactAction = _playerActionMap.FindAction("Interact", true);
        _playerActionMap.Enable();
    }

    private void OnEnable()
    {
        //re-enable after being disabled (first enable happens in Start)
        if (_playerActionMap != null)
        {
            _playerActionMap.Enable();
        }
    }

    private void OnDisable()
    {
        if (_playerActionMap != null)
        {
            _playerActionMap.Disable();
        }
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
        //(composite mode = Digital, so diagonal is not normalized, same as GetAxisRaw)
        Vector2 moveInput = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        _horizontalInput = moveInput.x;
        _verticalInput = moveInput.y;

        //set input value as a new Vector3
        Vector3 move = new Vector3(_horizontalInput, 0f, _verticalInput);

        //apply gravity when in the air
        //keep a small downward force when grounded so isGrounded stays stable
        if (_controller.isGrounded && _yVelocity < 0f)
        {
            _yVelocity = -1f;
        }
        else
        {
            _yVelocity -= _gravity * Time.deltaTime;
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
        if (_sprintAction != null && _sprintAction.WasPressedThisFrame())
        {
            _speed *= 2f;
        }
        else if (_sprintAction != null && _sprintAction.WasReleasedThisFrame())
        {
            _speed /= 2f;
        }

        //horizontal movement plus vertical (gravity) velocity
        Vector3 velocity = move * _speed;
        velocity.y = _yVelocity;
        _controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Item")
        {
            if(_interactAction != null && _interactAction.IsPressed() && Time.time > _canPickUp)
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
