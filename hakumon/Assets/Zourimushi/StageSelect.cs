using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    public void Stage1A()
    {
        SceneManager.LoadScene("1-1");
    }

    public void Stage1B()
    {
        SceneManager.LoadScene("1-2");
    }

    public void Stage1C()
    {
        SceneManager.LoadScene("1-3");
    }

    public void Stage1D()
    {
        //SceneManager.LoadScene("1-4");
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
