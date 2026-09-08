using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)  //触れたかをチェック
    {
        if(other.CompareTag("Player"))  //相手がplayerか
        {
            Destroy(gameObject);
        }
    }
}
