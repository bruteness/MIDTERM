using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _gravity;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _health;
    [SerializeField] private float _maxStamina;
    [SerializeField] private float _staminaDecreaseSpeed;
    [SerializeField] private float _staminaIncreaseSpeed;
    [SerializeField] private bool _hurtplayer;

    private CharacterController _controller;
    private GameManager _gameManager;
    private Animator _animator;
    private Vector3 _velocity;
    private bool _groundedPlayer;
    private bool _canRun;
    private float _stamina;
    private float _jumpHeight = 1.0f;
    private float _resetTimer = 3.0f;
    private float _idleTimer;

    public int Health { get { return _health; } }
    public int MaxHealth { get { return _maxHealth; } }
    public float Stamina { get { return _stamina; } }
    public float MaxStamina { get { return MaxStamina; } }

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _gameManager = FindObjectOfType<GameManager>();
        _idleTimer = _resetTimer;
        _stamina = _maxStamina;
        _gameManager.DisplayHealth(Health);
    }

    void Update()
    {
        //Always display the current stamina in the bar
        _gameManager.DisplayStamina(_stamina);

        //Let the player move as long as the game is in the play state
        if (_gameManager.State == GameManager.GameState.Play)
        {
            //*For testing purposes. Would not keep in the final product.*
            //If the bool is pressed in the inspector, it will hurt the player, then reset itself to off so it can be
            //tested again quickly
            if (_hurtplayer)
            {
                DecreaseHealth();
                _hurtplayer = false;
            }
            
            //Allow the player to run if they have enough stamina
            _canRun = _stamina <= 1 ? false : true;

            //Handle the movement for the player
            Movement();

            if (Input.GetButton("Jump") && _groundedPlayer) //Predefined jump in unity mapped to the space bar
            {
                Jump();
            }
        }

        //Play the lose animation and stop the player movement
        else if (_gameManager.State == GameManager.GameState.Lose)
        {
            _animator.SetTrigger("Die");
        }
        //Play the win animation and stop the player movement
        else if (_gameManager.State == GameManager.GameState.Win)
        {
            _animator.SetTrigger("Win");
        }
        _velocity.y += _gravity * Time.deltaTime; //Setting velocity in the y direction to the acceleration of gravity in relation to our fps (Time.deltaTime)
        _controller.Move(_velocity * Time.deltaTime); //Movement based on velocity
    }

    private void Movement()
    {
        _groundedPlayer = _controller.isGrounded; //Was the character touching the ground during the last frame? Accessing character controller's isGrounded property

        if (_groundedPlayer && _velocity.y < 0)
        {
            _velocity.y = 0f; //If the character was grounded in the last frame and is now moving in a negative velocity (falling down), set the velocity (speed and direction) to zero
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //Predefined axes in Unity linked to WASD controllers
        move = transform.TransformDirection(move);

        _controller.Move(move * Time.deltaTime * _moveSpeed); //Moves character in the given direction from our move vector3

        _velocity.y += _gravity * Time.deltaTime; //Setting velocity in the y direction to the acceleration of gravity in relation to our fps (Time.deltaTime)
        _controller.Move(_velocity * Time.deltaTime); //Movement based on velocity

        HandleMoveAniamtions(move);
    }

    private void HandleMoveAniamtions(Vector3 move)
    {
        if (Input.GetKey(KeyCode.S) && move != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            IncreaseStamina();
            StopLongIdle();
            WalkBackwards();
        }
        else if (move != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            IncreaseStamina();
            StopLongIdle();
            Walk();
        }
        else if (Input.GetKey(KeyCode.S) && move != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            DecreaseStamina();
            StopLongIdle();
            if(_canRun)
            {
                RunBackwards();
            }
            else
            {
                WalkBackwards();
            }
        }
        else if (move != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            DecreaseStamina();
            StopLongIdle();
            if(_canRun)
            {
                Run();
            }
            else
            {
                Walk();
            }
        }
        else if (move == Vector3.zero)
        {
            IncreaseStamina();
            Idle();

            //Play the animation for waiting around 
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

    private void DecreaseStamina()
    {
        _stamina -= _staminaDecreaseSpeed * Time.deltaTime;
        _stamina = _stamina <= 0 ? 0 : _stamina;
    }

    private void IncreaseStamina()
    {
        _stamina += _staminaIncreaseSpeed * Time.deltaTime;
        _stamina = _stamina > _maxStamina ? _maxStamina : _stamina;
    }

    //Set the speed and animation to Walk
    private void Walk()
    {
        _moveSpeed = _walkSpeed;
        _animator.SetFloat("Speed", .50f, .1f, Time.deltaTime);
    }
    //Set the speed and animation to Walk Backwards
    private void WalkBackwards()
    {
        _moveSpeed = _walkSpeed;
        _animator.SetFloat("Speed", -.50f, .1f, Time.deltaTime);
    }
    //Set the speed and animation to Run
    private void Run()
    {
        _moveSpeed = _runSpeed;
        _animator.SetFloat("Speed", 1f, .1f, Time.deltaTime);
    }
    //Set the speed and animation to Run Backwards
    private void RunBackwards()
    {
        _moveSpeed = _runSpeed;
        _animator.SetFloat("Speed", -1f, .1f, Time.deltaTime);
    }
    //Set the animation to idle
    private void Idle()
    {
        _animator.SetFloat("Speed", 0.001f, .1f, Time.deltaTime);
    }
    //Set the animation to a different idle animtion if the player has not moved in a certain
    //amount of time
    private void StartLongIdle()
    {
        _animator.SetBool("isHoldingIdle", true);
    }
    //Turn off the boolean for the idle animation, so that it stops the animation
    private void StopLongIdle()
    {
        _idleTimer = _resetTimer;
        _animator.SetBool("isHoldingIdle", false);
    }

    private void Jump()
    {
        _velocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravity); //Change velocity to represent a jumping behavior
    }
    //Add one health to the players current HP and then display it on the screen using the GameManager
    public void IncreaseHealth()
    {
        _health++;
        _health = _health > 4 ? 4 : _health; //Do not allow for the health to go past 4 for the player
        _gameManager.DisplayHealth(Health);
    }
    //Take one health away from the players current HP, then display it on the screen using the GameManager unless 
    //the players current HP is equal to or less than zero
    public void DecreaseHealth()
    {
        _health--;
        if (_health <= 0)
        {
            _health = 0;
            _gameManager.State = GameManager.GameState.Lose;
        }
        _gameManager.DisplayHealth(Health);
    }
}