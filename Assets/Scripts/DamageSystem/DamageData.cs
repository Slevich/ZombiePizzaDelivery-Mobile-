using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData
{
    #region Properties
    public int Amount { get; set; }
    public DamageDealer Source { get; set; }
    public IDamageable Target { get; set; }
    public Vector3 HitPoint { get; set; }
    public object AdditionalData { get; set; }
    #endregion

    #region Constructor
    public DamageData(int DamageAmount, DamageDealer DamageSource, Vector3 HitCollisionPoint, IDamageable DamageTarget, object[] AdditionalDamageInfo = null)
    {
        Amount = DamageAmount;
        Source = DamageSource;
        HitPoint = HitCollisionPoint;
        Target = DamageTarget;
        AdditionalData = AdditionalDamageInfo;
    }
    #endregion
}
