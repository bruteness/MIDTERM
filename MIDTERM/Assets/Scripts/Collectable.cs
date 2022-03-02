using UnityEngine;

public class Collectable : MonoBehaviour
{
    [TagSelector][SerializeField] private string _collectableTag;
    [TagSelector][SerializeField] private string _healthTag;
    [TagSelector][SerializeField] private string _playerTag;

    private void OnTriggerEnter(Collider other) {
        // Check for collectable pickup, add to collected amount, then destroy the object
        if(other.tag == _playerTag && this.tag == _collectableTag)
        {
            FindObjectOfType<GameManager>().GetCoin();
            Destroy(gameObject);
        }
        // Check for health pickup, add to the players health, then destroy the object
        else if(other.tag == _playerTag && this.tag == _healthTag)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.IncreaseHealth();
            FindObjectOfType<GameManager>().DisplayHealth(player.Health);
            Destroy(gameObject);
        }
    }
}