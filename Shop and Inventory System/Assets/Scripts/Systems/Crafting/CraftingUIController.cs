using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class CraftingUIController : MonoBehaviour
{
    [SerializeField] private List<CraftSkill> skills;
    [SerializeField] private PlayerStats playerStats;
    
    private CustomProgressBar durabilityProgressBar;
    private CustomProgressBar cpProgressBar;
    private CustomProgressBar progressProgressBar;
    private CustomProgressBar qualityProgressBar;
    
    private VisualElement root;
    private VisualElement icon;
    private VisualElement skillToolTip;

    private VisualElement skillOne;
    private VisualElement skillTwo;
    private VisualElement skillThree;
    private VisualElement skillFour;
    
    private Label stepText;
    private Label currentDurabilityText;
    private Label currentProgressText;
    private Label currentQualityText;

    private Label currentCPText;
    private Label maxCPText;
    
    private UIDocument document;
    private IShop activeShop;

    public Item item;

    private int step = 0;
    private bool craftSuccess;
    private bool craftFailed;
    
    private void Awake()
    {
        document = GetComponent<UIDocument>();
        //document.enabled = false;
        InitializeMenu();
    }

    private void Update()
    {
        if (durabilityProgressBar.maxValue == durabilityProgressBar.currentValue 
            || progressProgressBar.maxValue == progressProgressBar.currentValue)
        {
            if (progressProgressBar.currentValue == progressProgressBar.maxValue && !craftSuccess)
            {
                Debug.Log("Craft Success");
                craftSuccess = true;
            }
            else if(progressProgressBar.currentValue != progressProgressBar.maxValue && !craftFailed)
            {
                Debug.Log("Craft Failed");
                craftFailed = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartSkill(null, 0);
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartSkill(null, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartSkill(null, 2);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartSkill(null, 3);
        }
    }

    public void InitializeMenu()
    {
        //Store the root from the UI Document component
        root = document.rootVisualElement;

        if (root == null)
            return;

        SerializedObject so = new SerializedObject(item);
        root.Bind(so);

        icon = root.Q<VisualElement>("Icon");
        icon.style.backgroundImage = item.icon.texture;
        
        stepText = root.Q<Label>("StepText");
        skillToolTip = root.Q<VisualElement>("CraftingSkillToolTip");
        
        skillOne = root.Q<VisualElement>("SkillOne");
        skillTwo = root.Q<VisualElement>("SkillTwo");
        skillThree = root.Q<VisualElement>("SkillThree");
        skillFour = root.Q<VisualElement>("SkillFour");
        
        //durability
        durabilityProgressBar = new CustomProgressBar(0, item.craftingMaxDurability, new Color(0.84f, 0.01f, 0.31f, 1));
        root.Q<VisualElement>("DurabilityProgress").Add(durabilityProgressBar);
        currentDurabilityText = root.Q<Label>("CurrentDurabilityText");

        //cp
        cpProgressBar = new CustomProgressBar(1f, playerStats.cp, new Color(0.84f, 0.01f, 0.31f, 1));
        root.Q<VisualElement>("CPContent").Add(cpProgressBar);
        currentCPText = root.Q<Label>("CurrentCPText");
        maxCPText = root.Q<Label>("MaxCPText");
        currentCPText.text = cpProgressBar.currentValue.ToString();
        maxCPText.text = playerStats.cp.ToString();
        
        //progress
        progressProgressBar = new CustomProgressBar(0, item.craftingMaxProgress, new Color(0.99f, 0.97f, 0.65f, 1));
        root.Q<VisualElement>("ProgressBarContainer").Insert(0, progressProgressBar);
        currentProgressText = root.Q<Label>("CurrentProgressText");
        
        //quality
        qualityProgressBar = new CustomProgressBar(0, item.craftingMaxQuality, new Color(0.99f, 0.49f, 0.13f, 1));
        root.Q<VisualElement>("QualityBarContainer").Insert(0, qualityProgressBar);
        currentQualityText = root.Q<Label>("CurrentQualityText");
        
        //events
        skillOne.RegisterCallback<PointerMoveEvent>(ev => SkillHover(ev, 0));
        skillTwo.RegisterCallback<PointerMoveEvent>(ev => SkillHover(ev, 1));
        skillThree.RegisterCallback<PointerMoveEvent>(ev => SkillHover(ev, 2));
        skillFour.RegisterCallback<PointerMoveEvent>(ev => SkillHover(ev, 3));
        skillOne.RegisterCallback<PointerOutEvent>(SkillHoverLeave);
        skillTwo.RegisterCallback<PointerOutEvent>(SkillHoverLeave);
        skillThree.RegisterCallback<PointerOutEvent>(SkillHoverLeave);
        skillFour.RegisterCallback<PointerOutEvent>(SkillHoverLeave);
        
        skillOne.RegisterCallback<PointerDownEvent>(ev => StartSkill(ev, 0));
        skillTwo.RegisterCallback<PointerDownEvent>(ev => StartSkill(ev, 1));
        skillThree.RegisterCallback<PointerDownEvent>(ev => StartSkill(ev, 2));
        skillFour.RegisterCallback<PointerDownEvent>(ev => StartSkill(ev, 3));
    }

    private void StartSkill(PointerDownEvent ev, int skillIndex)
    {
        if(durabilityProgressBar.maxValue == durabilityProgressBar.currentValue 
           || progressProgressBar.maxValue == progressProgressBar.currentValue)
            return;

        CraftSkill skill = skills[skillIndex];
        
        Random random = new Random();
        float percent = random.Next(0, 100);
        
        if (skillIndex == 0)
        {
            UpdateProgress(item.craftingMaxProgress * 0.3f);
        }
        
        if (skillIndex == 1)
        {
            if(UpdateCP(-skill.skillCP))
                UpdateQuality(item.craftingMaxQuality * 0.2f);
            else return;
        }

        if (skillIndex == 2)
        {
            if(UpdateCP(-skill.skillCP))
                UpdateDurability(-30);
            else return;
        }

        if (skillIndex == 3)
        {
            if (UpdateCP(-skill.skillCP))
            {
                if (percent < skill.successRate100)
                {
                    UpdateQuality(item.craftingMaxQuality * 0.4f);
                    UpdateCP(skill.skillCP);
                }
                else
                {
                    Debug.Log("Failed Skill");
                }

            }
            else return;
        }
        
        step++;
        stepText.text = "Step " + step.ToString();
        UpdateDurability(10);
    }

    private void UpdateDurability(float value)
    {
        durabilityProgressBar.currentValue += value;
        durabilityProgressBar.UpdateProgressBar();
        currentDurabilityText.text = durabilityProgressBar.currentValue.ToString();
    }
    
    private bool UpdateCP(float value)
    {
        if(cpProgressBar.currentValue + value < 0)
            return false;
        
        cpProgressBar.currentValue += value;
        cpProgressBar.UpdateProgressBar();
        currentCPText.text = cpProgressBar.currentValue.ToString();
        return true;
    }

    private void UpdateProgress(float value)
    {
        progressProgressBar.currentValue += value;
        progressProgressBar.UpdateProgressBar();
        currentProgressText.text = progressProgressBar.currentValue.ToString();
    }
    
    private void UpdateQuality(float value)
    {
        qualityProgressBar.currentValue += value;
        qualityProgressBar.UpdateProgressBar();
        currentQualityText.text = qualityProgressBar.currentValue.ToString();
    }

    private void SkillHover(PointerMoveEvent evt, int skillIndex)
    {
        CraftSkill skill = skills[skillIndex];

        SerializedObject so = new SerializedObject(skill);
        skillToolTip.Bind(so);
        
        skillToolTip.style.visibility = Visibility.Visible;
        skillToolTip.style.display = DisplayStyle.Flex;
        
        //add offset to avoid layered visual element detection
        skillToolTip.style.top = evt.position.y + 5f;
        skillToolTip.style.left = evt.position.x + 5f;
    }
    
    private void SkillHoverLeave(PointerOutEvent evt)
    {
        skillToolTip.style.visibility = Visibility.Hidden;
        skillToolTip.style.display = DisplayStyle.None;
        skillToolTip.Unbind();
    }
}
