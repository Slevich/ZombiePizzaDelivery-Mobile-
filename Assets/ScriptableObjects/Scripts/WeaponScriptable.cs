using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptable", menuName = "ScriptableObjects/WeaponScriptableObject", order = 1)]
public class WeaponScriptable : ScriptableObject
{
    [field: Header("Weapon name."), SerializeField] public string Name { get; set; }
    [field: Header("Bullet prefab."), SerializeField] public GameObject Bullet { get; set; }
    [field: Header("Bullet movement speed modifier."), Range(0, 10), SerializeField] public float BulletSpeed { get; set; } = 1f;
    [field: Header("Bullet damage."), Range(0, 100), SerializeField] public int Damage { get; set; } = 10;
    [field: Header("Bullets per minute."), Range(0, 1000), SerializeField] public int FireRate { get; set; }
    [field: Header("Firing mode."), SerializeField] public FiringMode Mode { get; set; }
    [field: Header("Audio clip playing during shoot."), SerializeField] public AudioClip ShootSound { get; set; }
}

public enum FiringMode
{
    Single,
    Auto
}