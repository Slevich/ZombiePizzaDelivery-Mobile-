using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamageContainer : MonoBehaviour
{
    #region Properties
    [field: Header("Damage amount."), SerializeField, ReadOnly] public int Damage { get; set; }
    #endregion
}
