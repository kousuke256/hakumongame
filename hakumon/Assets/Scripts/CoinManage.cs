using UnityEngine;
using TMPro;

public class CoinManage : MonoBehaviour
{
    public int CoinCount = 0;
    public TextMeshProUGUI CoinText;
    void Start()
    {
        UpdateCoinText();
    }

    public void AddCoin()
    {
        CoinCount++;

        UpdateCoinText();
    }

    // Update is called once per frame
    void UpdateCoinText()
    {
        CoinText.text = "Coins: " + CoinCount;
    }
}