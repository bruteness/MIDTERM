using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _gravity;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _health;
    [SerializeField] private bool _hurtplayer;

    private CharacterController _controller; //Add character controller component to player
    private GameManager _gameManager;
    private Animator _animator;
    private Vector3 _velocity;
    private bool _groundedPlayer;
    private float _jumpHeight = 1.0f;
    private float _resetTimer = 3.0f;
    private float _idleTimer;

    public int Health { get { return _health; } }
    public int MaxHealth { get { return _maxHealth; } }

    void Start()
    {
        _controller = GetComponent<CharacterController>(); //reference to the character controller component
        _animator = GetComponent<Animator>();
        _gameManager = FindObjectOfType<GameManager>();
        _idleTimer = _resetTimer;
        _health = _maxHealth;
        _gameManager.DisplayHealth(Health);
    }

    void Update()
    {
        if(_gameManager.State == GameManager.GameState.Play)
        {
            if (_hurtplayer)
            {
                DecreaseHealth();
                _hurtplayer = false;
            }
            Movement();

            if (Input.GetButton("Jump") && _groundedPlayer) //predefined jump in unity mapped to the space bar
            {
                Jump();
            }
        }
        else if(_gameManager.State == GameManager.GameState.Lose)
        {
            _animator.SetTrigger("Die");
        }
        else if(_gameManager.State == GameManager.GameState.Win)
        {
            _animator.SetTrigger("Win");
        }
        _velocity.y += _gravity * Time.deltaTime; //setting velocity in the y direction to the acceleration of gravity in relation to our fps (Time.deltaTime)
        _controller.Move(_velocity * Time.deltaTime); //Movement based on velocity
    }

    private void Movement()
    {
        _groundedPlayer = _controller.isGrounded; //waws the character touching the ground during the last frame? Accessing character controller's isGrounded property

        if (_groundedPlayer && _velocity.y < 0)
        {
            _velocity.y = 0f; //if the character was grounded in the last frame and is now moving in a negative velocity (falling down), set the velocity (speed and direction) to zero
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //predefined axes in Unity linked to WASD controllers
        move = transform.TransformDirection(move);

        _controller.Move(move * Time.deltaTime * _moveSpeed); //moves character in the given direction from our move vector3

        _velocity.y += _gravity * Time.deltaTime; //setting velocity in the y direction to the acceleration of gravity in relation to our fps (Time.deltaTime)
        _controller.Move(_velocity * Time.deltaTime); //Movement based on velocity

        if (Input.GetKey(KeyCode.S) && move != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            StopLongIdle();
            WalkBackwards();
        }
        else if (move != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            StopLongIdle();
            Walk();
        }
        else if (Input.GetKey(KeyCode.S) && move != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            StopLongIdle();
            RunBackwards();
        }
        else if (move != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            StopLongIdle();
            Run();
        }
        else if (move == Vector3.zero)
        {
            Idle();

            // Play the animation for waiting around 
            if (_idleTimer > 0)
            {
                _idleTimer -= Time.deltaTime;
            }
            else
            {
                StartLongIdle();
            }
        }
    }

    // Set the speed and animation to Walk
    private void Walk()
    {
        _moveSpeed = _walkSpeed;
        _animator.SetFloat("Speed", .50f, .1f, Time.deltaTime);
    }
    private void WalkBackwards()
    {
        _moveSpeed = _walkSpeed;
        _animator.SetFloat("Speed", -.50f, .1f, Time.deltaTime);
    }
    // Set the speed and animation to Run
    private void Run()
    {
        _moveSpeed = _runSpeed;
        _animator.SetFloat("Speed", 1f, .1f, Time.deltaTime);
    }
    private void RunBackwards()
    {
        _moveSpeed = _runSpeed;
        _animator.SetFloat("Speed", -1f, .1f, Time.deltaTime);
    }
    private void Idle()
    {
        _animator.SetFloat("Speed", 0.001f, .1f, Time.deltaTime);
    }
    private void StartLongIdle()
    {
        _animator.SetBool("isHoldingIdle", true);
    }
    private void StopLongIdle()
    {
        _idleTimer = _resetTimer;
        _animator.SetBool("isHoldingIdle", false);
    }
    private void Jump()
    {
        _velocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravity); //Change velocity to represent a jumping behavior
    }
    public void IncreaseHealth()
    {
        _health++;
        _health = _health > 4 ? 4 : _health; // Do not allow for the health to go past 4 for the player
        _gameManager.DisplayHealth(Health);
    }
    public void DecreaseHealth()
    {
        _health--;
        if(_health <= 0)
        {
            _health = 0;
            _gameManager.State = GameManager.GameState.Lose;
        }
        _gameManager.DisplayHealth(Health);
    }
}