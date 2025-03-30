using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    #region Fields
    [Header("Max (start) health."), SerializeField, Range(1, 1000)] private int _maxHealth = 100;
    [Header("Current health."), SerializeField] private int _currentHealth = 100;
    [Header("Event on current health is 0."), SerializeField] private UnityEvent _onHealthOver;
    [Header("Event on current health changed."), SerializeField] private UnityEvent<float> _onHealthChangePercantage;
    [Header("OnDamage event."), SerializeField] private UnityEvent _onDamage;
    [Header("OnHeal event."), SerializeField] private UnityEvent _onHeal;
    #endregion

    #region Properties
    public float HealthPercentage { get; private set; } = 1f;
    private bool _healthIsOver => _currentHealth == 0;
    private bool _healthIsMax => _currentHealth == _maxHealth;
    #endregion

    #region Methods
    private void Awake ()
    {
        _currentHealth = _maxHealth;
        HealthPercentage = MathF.Round(_currentHealth / _maxHealth, 2);
    }

    private void ChangeHealth(int FloatingSignValue)
    {
        int newHealthValue = _currentHealth + FloatingSignValue;

        if (newHealthValue < 0)
            _currentHealth = 0;
        else if(newHealthValue > _maxHealth)
            _currentHealth = _maxHealth;
        else
            _currentHealth = newHealthValue;

        float healthPercantageUnrounded = (float)_currentHealth / (float)_maxHealth;
        HealthPercentage = MathF.Round(healthPercantageUnrounded, 2);
        _onHealthChangePercantage?.Invoke(HealthPercentage);
    }

    public void Damage(int DamageValue)
    {
        if (_healthIsOver)
            return;

        ChangeHealth(-DamageValue);
        _onDamage?.Invoke();

        if (_healthIsOver)
            _onHealthOver?.Invoke();
    }

    public void Heal(int HealValue)
    {
        if (_healthIsMax)
            return;

        ChangeHealth(+HealValue);
        _onHeal?.Invoke();
    }

    public bool RespondOnHeal () => !_healthIsMax;
    #endregion
}
