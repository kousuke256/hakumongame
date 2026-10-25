

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Staffroll : MonoBehaviour
{
    public float speed = 2f;
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
        transform.position += transform.up * speed * Time.deltaTime;
    }

    IEnumerator StartGame()
    {
        audioSource.PlayOneShot(Sample);

        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene("StageSelect");
    }
}

