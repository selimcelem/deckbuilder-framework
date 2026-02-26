using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform visualRoot;

    public float liftAmount = 120f;
    public float speed = 12f;

    private bool hovering;

    private void Awake()
    {
        visualRoot = transform.parent.Find("VisualRoot") as RectTransform;
    }

    private void Update()
    {
        if (!visualRoot) return;

        Vector3 target = hovering ? Vector3.up * liftAmount : Vector3.zero;

        visualRoot.localPosition = Vector3.Lerp(
            visualRoot.localPosition,
            target,
            Time.deltaTime * speed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}