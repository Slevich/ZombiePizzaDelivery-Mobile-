using System.Collections;
using System.Collections.Generic;
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
    public Vector2 ReturnInputDirection ()
    {
        return Vector2.zero;
    }

    public InputActionPhase ReturnInputState ()
    {
        return InputActionPhase.Waiting;
    }
    #endregion
}
