using UnityEngine;
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
}
