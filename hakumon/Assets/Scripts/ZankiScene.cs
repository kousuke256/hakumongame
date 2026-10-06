using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTimer : MonoBehaviour
{
    // 何秒待つか
    public float waitTime = 3f;
    // 移動先のScene名
    public string nextSceneName = "Start";
    // このSceneが始まったときに1回だけ実行される
    void Start()
    {
        LifeManager.Instance.ZankiLife();
        // waitTime秒後にChangeSceneを実行する
        Invoke("ChangeScene", waitTime);
    }
    // Sceneを切り替える処理
    void ChangeScene()
    {
        LifeManager.Instance.NormalLife();
        // nextSceneNameで指定したSceneへ移動
        SceneManager.LoadScene(nextSceneName);
    }
}