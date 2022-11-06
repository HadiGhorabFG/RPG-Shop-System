using System;
using UnityEngine;

[CreateAssetMenu]
public class CraftSkill : ScriptableObject
{
    public string skillName;
    public string description;
    public int skillCP;
    public float successRate;

    [HideInInspector] public float successRate100;

    private void OnValidate()
    {
        successRate100 = successRate * 100;
    }
}
