using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PauseTextChangeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text;

    public Color normalColor = Color.white;
    public Color hoverColor = Color.red;

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
    }
}