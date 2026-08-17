using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class FloatingTipIcon : MonoBehaviour, IPointerClickHandler
{
    [Header("Float Animation")]
    [SerializeField] private float floatAmplitude = 6f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Popup")]
    [SerializeField] private TipPopupController tipPopup; // drag the popup's GameObject here

    private RectTransform rectTransform;
    private Vector2 startAnchoredPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startAnchoredPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        rectTransform.anchoredPosition = startAnchoredPos + new Vector2(0f, offsetY);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tipPopup != null)
        {
            tipPopup.Show();
        }
    }
}