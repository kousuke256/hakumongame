using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Mouse.current.leftButton.wasPressedThisFrame)
        {
            SceneManager.LoadScene("StageSelect");
        }
    }
}