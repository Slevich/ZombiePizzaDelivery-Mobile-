using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;

public class SupplyManager : MonoBehaviour
{
    #region Fields
    private Health _playerHealth;
    #endregion

    #region Properties
    #endregion

    private void Awake ()
    {
        _playerHealth = PlayerReferencesContainer.Instance.PlayerHealth;
    }

    public bool NeedRequest(ObtainableItem Item)
    {
        if(Item.ItemType != typeof(SupplyItem))
            return false;

        SupplyItem supplyItem = Item as SupplyItem;

        return typeof(SupplyType) switch
        {
            _ when supplyItem.SupplyType == SupplyType.Health => _playerHealth.RespondOnHeal(),

            var t => throw new NotSupportedException($"{t.Name} is not supported")
        };
    }

    public void DestributeSupply(ObtainableItem Item)
    {
        if (Item.ItemType != typeof(SupplyItem))
            return;

        SupplyItem supplyItem = Item as SupplyItem;

        switch(supplyItem.SupplyType)
        {
            case SupplyType.Health:
                _playerHealth.Heal(supplyItem.Value);
                break;

            case SupplyType.Ammo:
                break;
        }
    }

    private bool IsNotNull (object nullableObject) => nullableObject != null;
}
