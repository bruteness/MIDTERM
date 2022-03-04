using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField] private float _hurtPlayerWaitTime;
    private NavMeshAgent _navMeshAgent;
    private GameManager _gameManager;
    private bool _canHurtPlayer;

    void Start()
    {
        _canHurtPlayer = true;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update() 
    {
        if(_gameManager.State == GameManager.GameState.Play)
        {
            _navMeshAgent.destination = FindObjectOfType<PlayerController>().transform.position; //Always make the zombie chase the player if the game state is play
        }
        else
        {
            _navMeshAgent.ResetPath(); //Stop the zombie from moving
        }
    }

    //Initial hit for the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && _gameManager.State == GameManager.GameState.Play)
        {
            if(_canHurtPlayer)
            {
                HurtPlayer(other);
            }
        }
    }


    //Hurt the player if they stay in the hitbox
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && _gameManager.State == GameManager.GameState.Play)
        {
            if (_canHurtPlayer)
            {
                HurtPlayer(other);
            }
        }
    }

    private void HurtPlayer(Collider other)
    {
        other.GetComponent<PlayerController>().DecreaseHealth();
        StartCoroutine(WaitToHurtPlayer(_hurtPlayerWaitTime));
    }

    //Wait to be able to hit the player again
    public IEnumerator WaitToHurtPlayer(float waitTimeToHurt)
    {
        _canHurtPlayer = false;
        yield return new WaitForSeconds(waitTimeToHurt);
        _canHurtPlayer = true;
    }
}
