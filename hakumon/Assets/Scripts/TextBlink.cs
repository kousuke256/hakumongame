using UnityEngine;
using TMPro;

public class TextBlink : MonoBehaviour
{
    public TMP_Text text;

    // Update is called once per frame
    void Update()
    {
        Color c = text.color;
        c.a = Mathf.PingPong(Time.time * 2f, 1f);
        text.color = c;
    }
}
