using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DamageDealer : MonoBehaviour
{
    #region Properties
    [field: Header("Parent of the dealer."), SerializeField] public Transform Parent;
    [field: Header("Damage amount."), SerializeField, ReadOnly] public int Damage { get; set; }
    public Action OnCollideDamageable { get; set; }
    #endregion

    #region Methods
    private void OnTriggerEnter (Collider collider)
    {
        Health health = (Health)(ComponentsSearcher.GetComponentFromObject(collider.gameObject, typeof(Health)));

        if(health != null)
        {
            DamageData data = new DamageData
            (
                DamageAmount: Damage,
                DamageSource: this,
                HitCollisionPoint: transform.position,
                DamageTarget: health
            );
            PlayerReferencesContainer.Instance.DamageManager.AddNewData(data);
        }
    }
    #endregion
}
