using UnityEngine;
using TMPro;

public class LifeManager : MonoBehaviour
{
    public int lifeCount = 3;
    public TextMeshProUGUI lifeText;
    public static LifeManager Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        // PauseManagerのオブジェクトが既にあるなら削除する（pauseManagerが何個も作られないようにする対策）
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // sceneが切り替わってもオブジェクトを消さないメゾット
        DontDestroyOnLoad(gameObject);

    }
    void Start()
    {
        UpdateLifeText();
    }

    public void LoseLife()
    {
        lifeCount--;
        UpdateLifeText();
    }

    // Update is called once per frame
    void UpdateLifeText()
    {
        lifeText.text = "♥ ×" + lifeCount;
    }

    public void ResetLife()
    {
        lifeCount = 3;

        UpdateLifeText();
    }

    public void HideLife()
    {
        lifeText.gameObject.SetActive(false);
    }
    public void ShowLife()
    {
        lifeText.gameObject.SetActive(true);
    }
    // 残機表示を「Zankiシーン用」にする
    public void ZankiLife()
    {
        // 画面の真ん中に移動
        lifeText.rectTransform.anchoredPosition = Vector2.zero;
        // 文字を大きくする
        lifeText.fontSize = 150;
    }

    // 残機表示を通常の状態に戻す
    public void NormalLife()
    {
        // 左上に戻す
        lifeText.rectTransform.anchoredPosition = new Vector2(-200, 250);
        // 文字サイズを元に戻す
        lifeText.fontSize = 36;
    }
}
