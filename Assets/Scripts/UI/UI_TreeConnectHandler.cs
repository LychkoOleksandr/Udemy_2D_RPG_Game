using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectHandler childNode;
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length;
    [Range(-25, 25)] public float offset;
}

[ExecuteAlways]
public class UI_TreeConnectHandler : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnection[] connections;

    private Image connectionImage;
    private Color originalColor;

    private void Awake()
    {
        if (connectionImage)
        {
            originalColor = connectionImage.color;
        }
    }

    private void OnValidate()
    {
        if (connectionDetails.Length <= 0)
        {
            return;
        }
        
        if (connectionDetails.Length != connections.Length)
        {
            Debug.Log("Details length and connection length should be equal. - " + gameObject.name);
            return;
        }
        
        UpdateConnections();
    }

    public void UpdateConnections()
    {
        for (int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];
            var connection = connections[i];
            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            Image connectionImage = connection.GetConnectionImage();
            
            connection.DirectConnection(detail.direction, detail.length, detail.offset);

            if (!detail.childNode)
                continue;
            
            detail.childNode.SetPosition(targetPosition);
            detail.childNode.SetConnectionImage(connectionImage);
            detail.childNode.transform.SetAsLastSibling();
        }
    }

    public void UpdateAllConnections()
    {
        UpdateConnections();

        foreach (UI_TreeConnectDetails detail in connectionDetails)
        {
            if (!detail.childNode) continue;
            detail.childNode.UpdateConnections();
        }
    }

    public void UnlockConnectionImage(bool unlocked)
    {
        if (!connectionImage)
        {
            return;
        }
        
        connectionImage.color = unlocked ? Color.white : originalColor;
    }
    
    public void SetConnectionImage(Image image) => connectionImage = image;
    
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
