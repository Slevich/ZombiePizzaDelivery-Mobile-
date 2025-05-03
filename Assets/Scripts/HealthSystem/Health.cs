using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    #region Fields
    [field: Header("Tag of the damageable objects."), SerializeField] public HealthTags Tag { get; set; } = HealthTags.Damageable;
    [field: Header("Health stats info."), SerializeField] public HealthState HealthInfo { get; set; }
    [field: Header("Event on current health is 0."), SerializeField] public UnityEvent OnHealthIsOver { get; set; }
    [field: Header("Event on current health is equal max value."), SerializeField] public UnityEvent OnHealthIsMax { get; set; }
    [field: Header("Event on current health changed percentage."), SerializeField] public UnityEvent<float> OnHealthPercentageChanged { get; set; }
    [field: Header("Event on causing damage."), SerializeField] public UnityEvent OnCauseDamage { get; set; }
    [field: Header("Event on healing."), SerializeField] public UnityEvent OnHeal { get; set; }
    #endregion

    #region Properties
    public float HealthPercentage => HealthInfo.Percentage;
    private bool healthIsOver => HealthInfo.HealthIsOver;
    private bool healthIsMax => HealthInfo.HealthIsMax;
    #endregion

    #region Methods
    private void Awake () => HealthInfo.SetStartValues();

#if UNITY_EDITOR
    private void OnValidate ()
    {
        if(!Application.IsPlaying(this) && HealthInfo != null && !HealthInfo.HealthIsMax)
            HealthInfo.EquateCurrentToMaxHealth();
    }
#endif
    public void CauseDamage(int DamageValue)
    {
        if (healthIsOver)
            return;

        DamageValue = Math.Abs(DamageValue);
        HealthInfo.ChangeHealth(-DamageValue);
        OnCauseDamage?.Invoke();
        OnHealthPercentageChanged?.Invoke(HealthInfo.Percentage);

        if (healthIsOver)
            OnHealthIsOver?.Invoke();
    }

    public void Heal(int HealValue)
    {
        if (healthIsMax)
            return;

        HealValue = Math.Abs(HealValue);
        HealthInfo.ChangeHealth(+HealValue);
        OnHeal?.Invoke();
        OnHealthPercentageChanged?.Invoke(HealthInfo.Percentage);
    }

    public bool RequestToHeal () => !healthIsMax;
    public bool RequestToDamage () => !healthIsOver;
    #endregion
}
