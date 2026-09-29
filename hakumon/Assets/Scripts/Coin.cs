using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    public CoinManage coinManage;
    private void OnTriggerEnter2D(Collider2D other)  //触れたかをチェック
    {
        if(other.CompareTag("Player"))  //相手がplayerか
        {
            coinManage.AddCoin();

            Destroy(gameObject);
        }
    }
}
