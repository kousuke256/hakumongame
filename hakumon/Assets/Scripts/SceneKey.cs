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
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            LifeManager.Instance.ResetLife();

            LifeManager.Instance.ShowLife();

            SceneManager.LoadScene("Start");
        }
    }
}