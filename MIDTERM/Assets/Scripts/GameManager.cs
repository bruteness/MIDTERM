using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _collectableText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private GameObject _fill;
    [SerializeField] private GameObject _winText;
    [SerializeField] private GameObject _loseText;
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private float _waitTime;
    public static GameManager Instance{get; private set;}
    private GameState _state;
    private int _coinAmount;
    private int _playerMaxHealth;
    private bool _showingText; //Boolean so the text doesn't flicker on the screen too fast for the win/lose UI text

    public enum GameState { Win, Lose, Play }
    public GameState State { get { return _state; } set { _state = value; } }

    // Start is called before the first frame update
    void Start()
    {
        _collectableText.text = "Jewels: 0";
        _state = GameState.Play;
        _showingText = false;
    }
    
    void Update() 
    {
        switch(_state)
        {
            case GameState.Win:
                StartCoroutine(FlashText(_winText, _waitTime));
                Debug.Log("You Win!");
                break;
            case GameState.Lose:
                StartCoroutine(FlashText(_loseText, _waitTime));
                Debug.Log("You Lose.");
                break;
        }    
    }

    private IEnumerator FlashText(GameObject textObject, float waitTime)
    {
        if(!_showingText)
        {
            _showingText = true;
            textObject.SetActive(true);
            _restartButton.SetActive(true);
            yield return new WaitForSeconds(waitTime);
            textObject.SetActive(false);
            yield return new WaitForSeconds(waitTime);
            _showingText = false;
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

    public void DisplayStamina(float stamina)
    {
        _staminaSlider.value = stamina / 100;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
