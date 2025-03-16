using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyItem : ObtainableItem
{
    private void Awake ()
    {
        ItemType = this.GetType();
    }
}
