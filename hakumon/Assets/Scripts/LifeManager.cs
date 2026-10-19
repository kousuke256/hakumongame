using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public int lifeCount = 3;
    public TextMeshProUGUI lifeText;
    public static LifeManager Instance;
    public GameObject image;

    void Awake()
    {
        // シングルトン
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // sceneが切り替わってもオブジェクトを消さないメゾット
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    void Start()
    {
        UpdateLifeText();
    }

    // シーンが切り替わったときに呼ばれる
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "StageSelect")
        {
            image.SetActive(false);
        }
        else
        {
            image.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void LoseLife()
    {
        lifeCount--;
        UpdateLifeText();
    }

    public void GetLife()
    {
        lifeCount++;
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
