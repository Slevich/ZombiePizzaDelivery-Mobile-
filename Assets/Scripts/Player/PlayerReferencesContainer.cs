using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferencesContainer : MonoBehaviour
{
    private static PlayerReferencesContainer _instance;
    public static PlayerReferencesContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerReferencesContainer>();
            }

            return _instance;
        }
    }
    [field: Header("Input.")]
    [field: SerializeField] public InputHandlerLevelInstance InputHandler { get; set; }

    [field: Space(10),Header("ChangeMovementDirection.")]
    [field: SerializeField] public AxisPositionChanger PositionChanger { get; set; }
    [field: SerializeField] public AxisMover Mover { get; set; }
    [field: Space(10), Header("Items system.")]
    [field: SerializeField] public WeaponHoldersManager HoldersManager { get; set; }
    [field: Space(10), Header("Health system.")]
    [field: SerializeField] public Health PlayerHealth { get; set; }

    [field: Space(10), Header("Supply system.")]
    [field: SerializeField] public SupplyManager SupplyManager { get; set; }
    [field: Space(10), Header("Damage system.")]
    [field: SerializeField] public Shooter Shooter { get; set; }
    [field: SerializeField] public DamageManager DamageManager { get; set; }
}
