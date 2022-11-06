using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class CustomProgressBar : VisualElement
{
    private VisualElement root;
    private VisualElement progressbarBG;
    private VisualElement progressbar;
    
    public float maxValue = 0;
    public float currentValue = 0;
    
    public CustomProgressBar(float startProgress, float maxValue, Color color)
    {
        style.width = Length.Percent(100);
        
        root = new VisualElement();
        root.style.width = Length.Percent(100);

        progressbarBG = new VisualElement();
        progressbar = new VisualElement();
        
        Add(root);
        root.Add(progressbarBG);
        progressbarBG.Add(progressbar);
        
        AddUSSToElements();
        
        progressbar.style.width = Length.Percent(startProgress * 100);
        progressbar.style.backgroundColor = color;

        this.maxValue = maxValue;
        currentValue = this.maxValue * startProgress;
        
        void AddUSSToElements()
        {
            //Add USS style properties to the elements
            progressbarBG.AddToClassList("ProgressBarBG");
            progressbar.AddToClassList("ProgressBar");
        }
    }

    public void UpdateProgressBar()
    {
        if (maxValue == 0)
            return;
        
        if (currentValue >= maxValue)
        {
            currentValue = maxValue;
            progressbar.style.width = Length.Percent(100);
            return;
        }

        if (currentValue <= 0)
        {
            currentValue = 0;
            progressbar.style.width = Length.Percent(0);
            return;
        }

        progressbar.style.width = Length.Percent((currentValue / maxValue) * 100);
    }
}
