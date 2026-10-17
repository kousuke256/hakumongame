using UnityEngine;
using UnityEngine.SceneManagement;

public class SeceneKey : MonoBehaviour
{
    void Start()
    {
        LifeManager.Instance.HideLife();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            LifeManager.Instance.ResetLife();

            LifeManager.Instance.ShowLife();

            SceneManager.LoadScene("1-1");
        }
    }
}