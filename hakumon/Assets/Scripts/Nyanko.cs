using UnityEngine;
using UnityEngine.SceneManagement;

public class Nyanko : MonoBehaviour
{
    public Transform[] stagePoints;
    public string[] sceneNames;

    private int currentStage = 0;
    private Vector3 targetPos;

    void Start()
    {
        targetPos = stagePoints[currentStage].position;
        transform.position = targetPos;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            5f * Time.deltaTime
        );

        // Aキーで左
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentStage--;

            if (currentStage < 0)
                currentStage = 0;

            MoveCursor();
        }

        // Bキーで右
        if (Input.GetKeyDown(KeyCode.B))
        {
            currentStage++;

            if (currentStage >= stagePoints.Length)
                currentStage = stagePoints.Length - 1;

            MoveCursor();
        }

        // Enterで決定
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(sceneNames[currentStage]);
        }
    }

    void MoveCursor()
    {
        targetPos = stagePoints[currentStage].position;
    }
}