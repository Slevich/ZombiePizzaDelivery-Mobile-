using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    #region Fields
    public HealthTags Tag { get; set; }
    public HealthState HealthInfo { get; set; }
    #endregion

    #region Properties
    public UnityEvent OnHealthIsOver { get; set; }
    public UnityEvent OnHealthIsMax { get; set; }
    public UnityEvent OnCauseDamage { get; set; }
    public UnityEvent OnHeal { get; set; }
    public UnityEvent<float> OnHealthPercentageChanged { get; set; }
    #endregion

    #region Methods
    public void CauseDamage (int DamageAmount);
    public void Heal (int HealAmount);
    public bool RequestToHeal ();
    public bool RequestToDamage ();
    #endregion
}
