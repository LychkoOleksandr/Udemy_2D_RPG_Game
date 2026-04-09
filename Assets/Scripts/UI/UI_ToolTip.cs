using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private Vector2 offset =  new(300, 20);
    
    private RectTransform rect;

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        if (!show)
        {
            rect.position = new Vector2(9999, 9999);
            return;
        }
        
        UpdatePosition(targetRect);
    }
    
    private void UpdatePosition(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2f;
        float screenTop = Screen.height;
        float screenBottom = 0;
        Vector2 targetPosition = targetRect.position;
        
        targetPosition.x = screenCenterX > targetPosition.x? targetPosition.x + offset.x : targetPosition.x - offset.x;

        float verticalHalf = rect.sizeDelta.y / 2f;
        float topY = targetPosition.y + verticalHalf;
        float bottomY = targetPosition.y - verticalHalf;

        if (topY > screenTop)
            targetPosition.y = screenTop - verticalHalf - offset.y;
        else if (bottomY < screenBottom)
            targetPosition.y = screenBottom + verticalHalf + offset.y;
        
        rect.position = targetPosition;   
    }
    
    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
}
