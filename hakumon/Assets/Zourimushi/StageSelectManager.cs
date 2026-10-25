using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageSelectManager : MonoBehaviour
{
     private AudioSource audioSource;
     public AudioClip Sampl;
     public AudioClip Sample;
     public GameObject returnPanel;
     public GameObject warningPanel;
     public GameObject PressWCanvas;
    public GameObject[] stageSelectTexts;
    private Rigidbody2D[] stageSelectTextRb;
    public GameObject[] stageButtons;
    private Rigidbody2D[] stageRb;

    // 解放されている一番先のステージ番号
    public static int unlockedStage = 5;


    void Start()
    {
        // Stage1のチュートリアルの表示履歴をリセット
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.DeleteKey("TutorialSeen_1-1_" + i);
        }
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.DeleteKey("TutorialSeen_1-2_" + i);
        }
        for (int i = 0; i < 1; i++)
        {
            PlayerPrefs.DeleteKey("TutorialSeen_1-3_" + i);
        }

        // 配列の範囲を超えないようにする
        audioSource = GetComponent<AudioSource>();
        int count = Mathf.Min(unlockedStage, stageButtons.Length);

        // Rigidbody2Dの配列を用意
        stageSelectTextRb = new Rigidbody2D[stageSelectTexts.Length];
        stageRb = new Rigidbody2D[stageButtons.Length];

        for (int i = 0; i < stageButtons.Length; i++)
        {
            // 解放済みのステージだけ表示する
            stageButtons[i].SetActive(i < count);
        }

        //rigidBody2Dを取得
        for (int i = 0; i < stageSelectTexts.Length; i++)
        {
            stageSelectTextRb[i] = stageSelectTexts[i].GetComponent<Rigidbody2D>();
        }    
        for (int i = 0; i < count; i++)
        {
            stageRb[i] = stageButtons[i].GetComponent<Rigidbody2D>();
        }

        // 他のシーンから戻ってきた場合だけ表示
        if (ShowPanelOnReturn.showPanel)
        {
            Debug.Log("A");
            returnPanel.SetActive(true);

            // 一度表示したらフラグをリセット
            ShowPanelOnReturn.showPanel = false;

            Time.timeScale = 0f;
        }
        else
        {
            returnPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame && PressWCanvas.activeSelf)
        {
            PressWCanvas.SetActive(false);
            Time.timeScale = 1f;
        }
        
        if (Keyboard.current.enterKey.wasPressedThisFrame && returnPanel.activeSelf)
        {
            returnPanel.SetActive(false);
            PressWCanvas.SetActive(true);
        }

        if(Time.timeScale == 0f)
        return;

        if (Keyboard.current.wKey.isPressed)
        {
            audioSource.PlayOneShot(Sample);
            Vector2 powerStageBottun = Vector2.up * 0.8f;

            for (int i = 0; i < stageSelectTexts.Length; i++)
            {
                stageSelectTextRb[i].gravityScale = -1;
            }   
            for (int i = 0; i + 1 <= unlockedStage; i++)
            {
                stageRb[i].gravityScale = -1;
                stageRb[i].AddForce(powerStageBottun);
            }
            
            
        }

        if (Keyboard.current.sKey.isPressed)
        {
            Vector2 powerStageBottun = Vector2.down * 0.8f;

            audioSource.PlayOneShot(Sample);

            for (int i = 0; i < stageSelectTexts.Length; i++)
            {
                stageSelectTextRb[i].gravityScale = 1;
            }    
            for (int i = 0; i + 1 <= unlockedStage; i++)
            {
                stageRb[i].gravityScale = 1;
                stageRb[i].AddForce(powerStageBottun);
            }
        }

        
    }

    // ステージをクリアしたときに呼び出す
    public static void ClearStage(int currentStageNumber)
    {
        if (currentStageNumber >= unlockedStage)
        {
            unlockedStage = currentStageNumber + 1;
        }
    }

    public void Stage1A()
    {
        StartCoroutine(stage1());
    }

    public void Stage1B()
    {
        StartCoroutine(stage2());
    }

    public void Stage1C()
    {
        StartCoroutine(stage3());
    }

    public void Stage1D()
    {
        StartCoroutine(stage4());
    }

    public void StageEx()
    {
        audioSource.PlayOneShot(Sampl);
         warningPanel.SetActive(true);
         Time.timeScale = 0f;
    }

    public void ReturnToTitle()
    {
        StartCoroutine(Returntitle());
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator stage1()
     {
         audioSource.PlayOneShot(Sampl);

         yield return new WaitForSeconds(0.5f);
         SceneManager.LoadScene("1-1");

     }
     IEnumerator stage2()
     {
         audioSource.PlayOneShot(Sampl);

         yield return new WaitForSeconds(0.5f);
         SceneManager.LoadScene("1-2");

     }
     IEnumerator stage3()
     {
         audioSource.PlayOneShot(Sampl);

         yield return new WaitForSeconds(0.5f);
         SceneManager.LoadScene("1-3");

     }
     IEnumerator stage4()
     {
         audioSource.PlayOneShot(Sampl);

         yield return new WaitForSeconds(0.5f);
         SceneManager.LoadScene("1-4");

     }

     public void StageExYesda()
    {
        StartCoroutine(stageExYes());
    }
    IEnumerator stageExYes()
    {
        Time.timeScale = 1f;
        audioSource.PlayOneShot(Sampl);

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("ExStage");
    }

    public void StageExNo()
    {
        Time.timeScale = 1f;
        warningPanel.SetActive(false);
    }


     IEnumerator Returntitle()
     {
         audioSource.PlayOneShot(Sampl);

         yield return new WaitForSeconds(0.5f);
         SceneManager.LoadScene("Title");

     }
}
