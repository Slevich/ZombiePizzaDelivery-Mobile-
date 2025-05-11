using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerDamageDealer : MonoBehaviour, IStopMovement
{
    #region Properties
    [field: Header("Parent of the dealer."), SerializeField] public Transform Parent;
    [field: Header("Damage amount."), SerializeField, ReadOnly] public int Damage { get; set; }
    [field: Header("Object to spawn on destroy."), SerializeField] public GameObject[] ObjectToSpawnOnDestroy { get; set; }
    public Action OnStopMovement { get; set; }
    public Action<Vector3> OnCollideDamageable { get; set; }

    private DamageManager _damageManager;
    #endregion

    #region Methods
    private void Awake ()
    {
        _damageManager = PlayerReferencesContainer.Instance.DamageManager;
    }

    private void OnTriggerEnter (Collider collider)
    {
        Health health = (Health)(ComponentsSearcher.GetComponentFromObject(collider.gameObject, typeof(Health)));

        if(health != null)
        {
            if(health.Tag == HealthTags.Player || health.Tag == HealthTags.None)
                return;

            DamageData data = new DamageData
            (
                DamageAmount: Damage,
                DamageSource: this,
                HitCollisionPoint: transform.position,
                DamageTarget: health
            );
            OnCollideDamageable?.Invoke(transform.position);
            OnStopMovement?.Invoke();
            _damageManager.AddNewData(data);
        }
    }
    #endregion
}
