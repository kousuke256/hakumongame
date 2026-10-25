using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Staffroll : MonoBehaviour
{
    public float speed = 2f;

    private AudioSource audioSource;
    public AudioClip Sample;

    public Image skipGauge;

    public float skipHoldTime = 1f;

    private bool isChangingScene = false;
    private float holdTime = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (skipGauge != null)
        {
            skipGauge.fillAmount = 0f;
            skipGauge.gameObject.SetActive(false);
        }

        StartCoroutine(AutoSceneChange());
    }

    void Update()
    {
        bool holding =
            Input.GetKey(KeyCode.Space) ||
            Input.GetKey(KeyCode.Return);

        if (!isChangingScene)
        {
            if (holding)
            {
                holdTime += Time.deltaTime;

                if (skipGauge != null)
                {
                    skipGauge.gameObject.SetActive(true);
                    skipGauge.fillAmount = Mathf.Clamp01(holdTime / skipHoldTime);
                }

                if (holdTime >= skipHoldTime)
                {
                    StartCoroutine(StartGame());
                }
            }
            else
            {
                holdTime = 0f;

                if (skipGauge != null)
                {
                    skipGauge.fillAmount = 0f;
                    skipGauge.gameObject.SetActive(false);
                }
            }

            if (Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartCoroutine(StartGame());
            }
        }

        transform.position += transform.up * speed * Time.deltaTime;
    }

    IEnumerator AutoSceneChange()
    {
        yield return new WaitForSeconds(40f);

        if (!isChangingScene)
        {
            SceneManager.LoadScene("StageSelect");
        }
    }

    IEnumerator StartGame()
    {
        isChangingScene = true;

        if (audioSource != null && Sample != null)
        {
            audioSource.PlayOneShot(Sample);
        }

        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene("StageSelect");
    }
}