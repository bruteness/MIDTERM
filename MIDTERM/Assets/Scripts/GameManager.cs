using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _collectableText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private GameObject _fill;
    [SerializeField] private GameObject _winText;
    [SerializeField] private GameObject _loseText;
    private int _playerMaxHealth;
    private int _coinAmount;
    private GameState _state;

    public enum GameState { Win, Lose, Play }

    public GameState State { get { return _state; } set { _state = value; } }


    // Start is called before the first frame update
    void Start()
    {
        _collectableText.text = "Jewels: 0";
        _state = GameState.Play;
    }
    
    void Update() 
    {
        switch(_state)
        {
            case GameState.Win:
                _winText.SetActive(true);
                Debug.Log("You Win!");
                break;
            case GameState.Lose:
                _loseText.SetActive(true);
                Debug.Log("You Lose.");
                break;
        }    
    }

    // Update is called once per frame
    public void GetCoin()
    {
        _coinAmount++;
        _collectableText.text = "Jewels: " + _coinAmount.ToString();
        if(_coinAmount >= 5)
        {
            _state = GameState.Win;
        }
    }

    public void DisplayHealth(int health)
    {
        _healthText.text = health.ToString() + "  /  " + FindObjectOfType<PlayerController>().MaxHealth;
        _healthSlider.value = (float)health / 4;
        if (_healthSlider.value <= 0)
            _fill.SetActive(false);
        else
            _fill.SetActive(true);
    }
}
