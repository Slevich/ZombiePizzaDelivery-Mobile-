using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObtainableWeapon : ObtainableItem
{
    [field: Header("Grabbable object prefab."), SerializeField] public GameObject GrabbableWeaponItem { get; set; }

    private void Awake ()
    {
        ItemType = this.GetType();
    }

    private void OnValidate ()
    {
        if (GrabbableWeaponItem != null)
            Name = GrabbableWeaponItem.name;
        else
            Name = "Item";
    }
}
