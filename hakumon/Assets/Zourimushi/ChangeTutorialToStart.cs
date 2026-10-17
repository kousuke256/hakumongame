using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeTutorialToStart : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("1-1");
        }
    }
}
