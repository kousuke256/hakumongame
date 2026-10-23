using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] eventCanvas;
    public float[] eventTriggerX;

    private bool existCanvas = false;
    private Transform player;
    private int i = 0;

    // シーンごとにチュートリアルの表示履歴を管理する
    private string GetTutorialKey(int index)
    {
        return "TutorialSeen_"
            + SceneManager.GetActiveScene().name
            + "_" + index;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<Transform>();

        // 最初にすべてのCanvasを非表示にする
        foreach (GameObject canvas in eventCanvas)
        {
            canvas.SetActive(false);
        }

        // すでに見たチュートリアルをスキップする
        SkipSeenTutorials();
    }

    void Update()
    {
        // すべてのチュートリアルが終わったら終了
        if (i >= eventCanvas.Length)
        {
            Time.timeScale = 1f;
            Destroy(gameObject);
            return;
        }

        if (existCanvas)
        {
            // Enterキーまたは左クリックで閉じる
            bool enterPressed =
                Keyboard.current != null &&
                Keyboard.current.enterKey.wasPressedThisFrame;

            bool mousePressed =
                Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame;

            if (enterPressed || mousePressed)
            {
                // 表示を消して、表示済みとして保存する
                eventCanvas[i].SetActive(false);

                PlayerPrefs.SetInt(GetTutorialKey(i), 1);
                PlayerPrefs.Save();

                existCanvas = false;
                Time.timeScale = 1f;
                i++;

                // 次のチュートリアルが表示済みなら飛ばす
                SkipSeenTutorials();
            }
        }
        else
        {
            // プレイヤーがトリガー位置を越えたら表示
            if (player.position.x < eventTriggerX[i])
                return;

            eventCanvas[i].SetActive(true);
            existCanvas = true;

            // ゲームを一時停止
            Time.timeScale = 0f;
        }
    }

    // 表示済みのチュートリアルを飛ばす処理
    private void SkipSeenTutorials()
    {
        while (i < eventCanvas.Length &&
               PlayerPrefs.GetInt(GetTutorialKey(i), 0) == 1)
        {
            eventCanvas[i].SetActive(false);
            i++;
        }
    }
}
/*using UnityEngine;
using UnityEngine.InputSystem;
public class TutorialManager : MonoBehaviour
{
    public GameObject[] eventCanvas;
    public float[] eventTriggerX;
    private bool existCanvas = false;
    private Transform player;
    private int i = 0;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        if (existCanvas)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                // キャンパスの表示を消し、次のキャンパスの処理へ
                Destroy(eventCanvas[i]);
                existCanvas = false;
                Time.timeScale = 1f;
                i++;

                // すべてのキャンパスの表示が終わったらこのオブジェクトを消す
                if(i >= eventCanvas.Length)
                Destroy(gameObject);
            }
        }
        else
        {
            // イベントトリガーのx座標を越したらキャンパスを表示して時間の流れを止める
            if(player.position.x < eventTriggerX[i])
            return;

            eventCanvas[i].SetActive(true);
            existCanvas = true;
            Time.timeScale = 0f;
        }
    }
}*/
