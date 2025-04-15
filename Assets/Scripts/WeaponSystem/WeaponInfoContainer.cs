using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponInfoContainer : MonoBehaviour
{
    #region Properties
    [field: Header("Weapon sriptable object."), SerializeField] public WeaponScriptable Info { get; set; }
    [field: Header("Event on every single shot."), SerializeField] public UnityEvent OnShot { get; set; }
    #endregion
}
