using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HealthState
{
    #region Fields
    [Header("Max (start) health."), SerializeField, Range(1, 1000)] private int _maxHealth = 100;
    [Header("Current health."), ReadOnly, SerializeField] protected int _currentHealth = 100;
    #endregion

    #region Properties
    public float Percentage { get; private set; } = 1f;
    public bool HealthIsOver => _currentHealth == 0;
    public bool HealthIsMax => _currentHealth == _maxHealth;
    #endregion

    #region Methods
    public void ChangeHealth (int FloatingSignValue)
    {
        int newHealthValue = _currentHealth + FloatingSignValue;

        if (newHealthValue < 0)
            _currentHealth = 0;
        else if (newHealthValue > _maxHealth)
            _currentHealth = _maxHealth;
        else
            _currentHealth = newHealthValue;

        float healthPercantageUnrounded = (float)_currentHealth / (float)_maxHealth;
        Percentage = MathF.Round(healthPercantageUnrounded, 2);
    }

    public void SetStartValues()
    {
        EquateCurrentToMaxHealth();
        Percentage = MathF.Round(_currentHealth / _maxHealth, 2);
    }

    public void EquateCurrentToMaxHealth() => _currentHealth = _maxHealth;
    #endregion
}