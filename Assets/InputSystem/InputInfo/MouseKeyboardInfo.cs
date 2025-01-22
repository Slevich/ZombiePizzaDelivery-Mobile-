using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseKeyboardInfo : InputInfo, IInputInfo
{
    #region Fields
    #endregion

    #region Properties
    #endregion

    #region Constructor
    public MouseKeyboardInfo (InputActions Actions) : base (Actions)
    {

    }
    #endregion

    #region Methods
    public Vector2 ReturnInputDirection() => _inputActions.Positions.InputDirection.ReadValue<Vector2>();
    public InputActionPhase ReturnInputState() => _inputActions.Positions.InputDirection.phase;
    #endregion
}