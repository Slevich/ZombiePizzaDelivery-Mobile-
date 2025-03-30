using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsObtainer : MonoBehaviour
{
    private void OnTriggerEnter (Collider other)
    {
        if(!other.TryGetComponent<ObtainableItem>(out ObtainableItem obtainableItem))
        {
            return;
        }

        other.enabled = false;
        SelectObtainableItemBehaviour(obtainableItem);
    }

    private void SelectObtainableItemBehaviour(ObtainableItem obtainableItem)
    {
        ChooseObtainableItemRecipient(obtainableItem)?.Invoke();
        Destroy(obtainableItem.gameObject);
    }

    private Action ChooseObtainableItemRecipient (ObtainableItem item) => typeof(ObtainableItem) switch
    {
        _ when item.ItemType == typeof(ObtainableWeapon) =>
        delegate 
        {
            ItemHoldersManager holdersManager = PlayerReferencesContainer.Instance.HoldersManager;
            bool positiveGrabResponse = holdersManager.RespondOnGrab(item);
        
            if(!positiveGrabResponse)
            {
                item.OnInteractWithTransform(false, null, null);
            }
            else
            {
                item.OnInteractWithTransform(true, transform, delegate { holdersManager.GrabNewItem(item); });
            }
        },
        
        _ when item.ItemType == typeof(SupplyItem) =>
        delegate
        {
            SupplyManager supplyManager = PlayerReferencesContainer.Instance.SupplyManager;
            bool supplyIsNeed = supplyManager.NeedRequest(item);

            if (!supplyIsNeed)
            {
                item.OnInteractWithTransform(false, null, null);
            }
            else
            {
                item.OnInteractWithTransform(true, transform, delegate { supplyManager.DestributeSupply(item); });
            }
        },
        
        var t => throw new NotSupportedException($"{t.Name} is not supported")
    };
}
