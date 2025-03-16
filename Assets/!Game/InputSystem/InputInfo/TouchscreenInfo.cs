using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchscreenInfo : InputInfo, IInputInfo
{
    #region Fields
    #endregion

    #region Constructor
    public TouchscreenInfo(InputActions Actions) : base(Actions)
    {

    }
    #endregion

    #region Properties
    #endregion

    #region Methods
    public Vector2 ReturnInputDirection () => Vector2.zero;
    public InputActionPhase ReturnInputState () => InputActionPhase.Waiting;
    public bool FireButtonPerformed () => false;
    #endregion
}
