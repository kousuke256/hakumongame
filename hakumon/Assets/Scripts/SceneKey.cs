using UnityEngine;
using UnityEngine.SceneManagement;

public class SeceneKey : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            SceneManager.LoadScene("Start");
        }
    }
}