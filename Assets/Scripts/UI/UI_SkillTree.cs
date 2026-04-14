using System;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private int skillPoints;
    [SerializeField] private UI_TreeConnectHandler[] parentNodes;

    public Player_SkillManager skillManager {get; private set;}

    private void Awake()
    {
        skillManager = FindAnyObjectByType<Player_SkillManager>();
    }

    private void Start()
    {
        UpdateAllConnections();
    }

    [ContextMenu("Reset Skill Tree")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (UI_TreeNode node in skillNodes)
            node.Refund();
    }

    [ContextMenu("Update All Connections")]
    public void UpdateAllConnections()
    {
        foreach (UI_TreeConnectHandler node in parentNodes)
        {
            node.UpdateAllConnections();
        }
    }
    
    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;
    public void RemoveSkillPoints(int cost) =>  skillPoints -= cost;
    public void AddSkillPoints(int points) => skillPoints += points;
}
