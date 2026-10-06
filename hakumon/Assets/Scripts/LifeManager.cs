using UnityEngine;
using TMPro;

public class LifeManager : MonoBehaviour
{
    public int lifeCount = 3;
    public TextMeshProUGUI lifeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
}
