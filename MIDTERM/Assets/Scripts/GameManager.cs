using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _collectableText;
    [SerializeField] private TextMeshProUGUI _healthText;
    private int _coinAmount;

    // Start is called before the first frame update
    void Start()
    {
        _collectableText.text = "Jewels: 0";
    }

    // Update is called once per frame
    public void GetCoin()
    {
        _coinAmount++;
        _collectableText.text = "Jewels: " + _coinAmount.ToString();
    }

    public void DisplayHealth(int health)
    {
        _healthText.text = "Life: " + health.ToString();
    }
}
