using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonGlow : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public GameObject glow;

    void Start()
    {
        glow.SetActive(false);
    }

    // カーソルがボタンに入ったとき
    public void OnPointerEnter(PointerEventData eventData)
    {
        glow.SetActive(true);
    }

    // カーソルがボタンから出たとき
    public void OnPointerExit(PointerEventData eventData)
    {
        glow.SetActive(false);
    }
}