using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptable", menuName = "ScriptableObjects/WeaponScriptableObject", order = 1)]
public class WeaponScriptable : ScriptableObject
{
    [field: Header("Weapon name."), SerializeField] public string Name { get; set; }
    [field: Header("Bullet prefab."), SerializeField] public GameObject Bullet { get; set; }
    [field: Header("Bullet movement speed modifier."), SerializeField] public float BulletSpeed { get; set; } = 1f;
    [field: Header("Bullet damage."), SerializeField] public int Damage { get; set; }
    [field: Header("Firing mode."), SerializeField] public FiringMode Mode { get; set; }
    [field: Header("Bullets per minute."), SerializeField] public int FireRate { get; set; }
    [field: Header("Audio clip playing during shoot."), SerializeField] public AudioClip ShootSound { get; set; }
}

public enum FiringMode
{
    Single,
    Auto
}