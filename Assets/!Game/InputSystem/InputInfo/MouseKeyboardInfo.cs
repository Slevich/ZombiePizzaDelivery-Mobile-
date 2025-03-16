using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseKeyboardInfo : InputInfo, IInputInfo
{
    #region Fields
    private InputActionPhase _fireButtonState = InputActionPhase.Disabled;
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
    public bool FireButtonPerformed () => _inputActions.Buttons.Fire.phase == InputActionPhase.Performed;
    #endregion
}