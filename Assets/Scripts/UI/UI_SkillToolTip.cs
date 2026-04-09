using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI_SkillTree skillTree;
    private UI ui;
    
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;
    [Space]
    [SerializeField] private string metConditionsHex;
    [SerializeField] private string notMetConditionsHex;
    [SerializeField] private string importantInfoHex;
    [SerializeField] private Color exampleColor;
    [SerializeField] private string lockedSkillText = "You've taken a different path - this skill is now locked.";
    
    private Coroutine textEffectCo;

    protected override void Awake()
    {
        base.Awake();
        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>(true);
    }

    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);

        if (!show && textEffectCo != null)
        {
            StopCoroutine(textEffectCo);
        }
    }

    public void ShowToolTip(bool show, RectTransform rect, UI_TreeNode node)
    {
        base.ShowToolTip(show, rect);
        
        skillName.text = node.skillData.displayName;
        skillDescription.text = node.skillData.description;

        string skillLockedText = GetColoredText(importantInfoHex, lockedSkillText);
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.skillData.cost, node.neededNodes, node.conflictNodes);
        
        skillRequirements.text = requirements;
    }

    public void LockedSkillEffect()
    {
        if (textEffectCo != null)
            StopCoroutine(textEffectCo);

        Debug.Log("Locked Skill Effect");
        textEffectCo = StartCoroutine(TextBlinkEffectCo(skillRequirements, .15f, 3));
    }

    private IEnumerator TextBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.text = GetColoredText(notMetConditionsHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);
            
            text.text = GetColoredText(importantInfoHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);
        }
    } 

    private string GetRequirements(int skillCost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new();

        sb.AppendLine("Requirements:");
        
        string costColor = skillTree.EnoughSkillPoints(skillCost) ? metConditionsHex : notMetConditionsHex;

        sb.AppendLine(GetColoredText(costColor, $"- {skillCost} skill point(s)"));

        foreach (UI_TreeNode node in neededNodes)
        {
            string nodeColor = node.isUnlocked ?  metConditionsHex : notMetConditionsHex;
            sb.AppendLine(GetColoredText(nodeColor, node.skillData.displayName));
        }
        
        if (conflictNodes.Length <= 0) return sb.ToString();

        sb.AppendLine();
        sb.AppendLine(GetColoredText(importantInfoHex, "Locks out: "));
        
        foreach (UI_TreeNode node in conflictNodes)
        {
            sb.AppendLine(GetColoredText(importantInfoHex, $"- {node.skillData.displayName}"));
        }

        return sb.ToString();
    }
}
