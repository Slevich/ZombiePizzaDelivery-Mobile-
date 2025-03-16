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
       delegate { PlayerReferencesContainer.Instance.HoldersManager.TryToGrabNewItem(item); },

        var t => throw new NotSupportedException($"{t.Name} is not supported")
    };
}
