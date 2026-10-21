using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip Sample;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        audioSource.PlayOneShot(Sample);

        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene("StageSelect");
    }
}