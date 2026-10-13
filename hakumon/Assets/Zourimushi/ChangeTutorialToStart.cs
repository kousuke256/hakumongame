using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeTutorialToStart : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            SceneManager.LoadScene("1-1(Complete)");
        }
    }
}
