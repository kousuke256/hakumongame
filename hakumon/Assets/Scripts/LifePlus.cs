using UnityEngine;

public class LifePlus : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)  //触れたかをチェック
    {
        if (other.CompareTag("Player"))  //相手がplayerか
        {
            LifeManager.Instance.GetLife();

            Destroy(gameObject);
        }
    }
}
