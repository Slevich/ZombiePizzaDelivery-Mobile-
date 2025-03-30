using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyItem : ObtainableItem
{
    #region Fields
    [Header("Supplies to restore type."), SerializeField] private SupplyType _supplyType = SupplyType.Health;
    [Header("Value to restore supply."), SerializeField, Range(1, 1000)] private int _supplyValue = 10;
    #endregion

    #region Properties
    public int Value => _supplyValue;
    public SupplyType SupplyType => _supplyType;
    #endregion

    #region Methods
    private void OnValidate ()
    {
        if (_supplyValue < 1)
            _supplyValue = 1;
        else if (_supplyValue > 1000)
            _supplyValue = 1000;
    }

    private void Awake ()
    {
        ItemType = this.GetType();
    }
    #endregion
}

public enum SupplyType
{
    Health,
    Ammo
}