using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectHandler connectHandler;

    [Header("Unlock details")] 
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;
    public bool isUnlocked;
    public bool isLocked;
    
    [Header("Skill details")]   
    public Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private int skillCost;
    [SerializeField] private Image skillIcon;
    [SerializeField] private string lockedColorHex = "#979797";
    private Color lastColor;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        connectHandler = GetComponent<UI_TreeConnectHandler>();
        UpdateIconColor(GetColorByHex(lockedColorHex));
    }
    
    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNodes();
        
        skillTree.RemoveSkillPoints(skillData.cost);
        connectHandler.UnlockConnectionImage(true);
        
        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData.upgradeData);
    }

    public void Refund()
    {
        isLocked = false;
        isUnlocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));
        
        skillTree.AddSkillPoints(skillData.cost);
        connectHandler.UnlockConnectionImage(false);
    }
    
    private bool CanBeUnlocked()
    {
        if (!skillTree.EnoughSkillPoints(skillData.cost))
        {
            return false;   
        }
        
        if (isLocked || isUnlocked)
        {
            return false;
        }
        
        if (neededNodes.Any(node => !node.isUnlocked))
        {
            return false;
        }

        return !conflictNodes.Any(node => node.isUnlocked);
    }

    private void LockConflictNodes()
    {
        foreach (UI_TreeNode node in conflictNodes)
            node.isLocked = true;
    }

    private void UpdateIconColor(Color color)
    {
        if (!skillIcon)
        {
            return;
        }
        
        lastColor = skillIcon.color;
        skillIcon.color = color;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, this);
        
        if (!isUnlocked && !isLocked)
            ToggleNodeHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);

        if (!isUnlocked && !isLocked)
            ToggleNodeHighlight(false);
    }

    private void OnDisable()
    {
        UpdateIconColor(isUnlocked ? Color.white : GetColorByHex(lockedColorHex));
    }

    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * .9f; highlightColor.a = 1f;
        Color colorToApply = highlight ? highlightColor : lastColor;
        
        UpdateIconColor(colorToApply);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
        {
            Unlock();
        }
        else if (isLocked)
        {
            ui.skillToolTip.LockedSkillEffect(); 
        }
    }

    private Color GetColorByHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }
    
    private void OnValidate()
    {
        if (!skillData)
            return;
        
        skillName =  skillData.displayName;
        skillCost = skillData.cost;
        skillIcon.sprite = skillData.icon;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;
    }

}
