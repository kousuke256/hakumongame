using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class ImageBlink : MonoBehaviour
{
    public float interval = 0.5f; // 点滅する間隔（秒）
    private Image targetImage;
    private float timer;
    private bool visible = true;
    private Color originalColor;
    void Awake()
    {
        targetImage = GetComponent<Image>();
        originalColor = targetImage.color;
    }
    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            visible = !visible;
            Color color = originalColor;
            color.a = visible ? originalColor.a : 0f;
            targetImage.color = color;
        }
    }
}