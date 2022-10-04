using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreInteractNotification : MonoBehaviour, ITextNotification
{
    public string Message { get; set ; }

    private void Awake()
    {
        Message = "Press 'Space' to open shop!";
    }
}
