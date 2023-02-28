using UnityEngine;
using UnityEngine.EventSystems;

public class Bubble : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SendMessageUpwards("HandleBubbleClick", transform.GetSiblingIndex(), SendMessageOptions.DontRequireReceiver);
    }
}
