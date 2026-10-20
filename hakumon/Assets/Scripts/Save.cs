using UnityEngine;
using UnityEngine.SceneManagement;
public class SavePoint : MonoBehaviour
{
    public Transform respawnPoint;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        LifeManager manager = LifeManager.Instance;
        if (manager == null) return;
        manager.hasSavePoint = true;
        if (respawnPoint != null)
        {
            manager.respawnPosition = respawnPoint.position;
        }
        else
        {
            manager.respawnPosition = other.transform.position;
        }
        manager.savedSceneName =
            SceneManager.GetActiveScene().name;

        Destroy(gameObject);
    }
}