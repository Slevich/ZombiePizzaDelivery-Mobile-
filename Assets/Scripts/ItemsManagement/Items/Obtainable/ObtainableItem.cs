using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObtainableItem : ItemsBase
{
    public Type ItemType { get; set; }
    public Action<bool, Transform, Action> OnInteractWithTransform { get; set; }
}
