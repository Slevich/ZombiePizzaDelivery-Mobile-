using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ItemsBase : MonoBehaviour
{
    [field: Header("Name of the item."), SerializeField] public string Name { get; set; } = "Item"; 
}