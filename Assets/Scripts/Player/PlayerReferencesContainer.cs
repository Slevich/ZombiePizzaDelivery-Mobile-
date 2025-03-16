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
    [field: Header("Input")]
    [field: SerializeField] public InputHandlerLevelInstance InputHandler { get; set; }

    [field: Space(10),Header("ChangeMovementDirection")]
    [field: SerializeField] public AxisPositionChanger PositionChanger { get; set; }
    [field: SerializeField] public AxisMover Mover { get; set; }
    [field: Space(10), Header("Animations")]
    [field: SerializeField] public RotationAnimationsSequences GeneralAnimations { get; set; }
    [field: SerializeField] public RotationAroundAxisAnimation[] AxesSubanimations { get; set; }
    [field: Space(10), Header("Items system")]
    [field: SerializeField] public ItemHoldersManager HoldersManager { get; set; }
}
