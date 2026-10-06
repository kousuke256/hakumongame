using UnityEngine;
using TMPro;

public class CoinManage : MonoBehaviour
{
    public int coinCount = 0;
    public TextMeshProUGUI coinText;
    void Start()
    {
        UpdateCoinText();
    }

    public void AddCoin()
    {
        coinCount++;

        UpdateCoinText();
    }

    // Update is called once per frame
    void UpdateCoinText()
    {
        coinText.text = "Coins: " + coinCount;
    }
}