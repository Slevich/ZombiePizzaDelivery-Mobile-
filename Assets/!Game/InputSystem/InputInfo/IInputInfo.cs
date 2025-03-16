using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputInfo
{
    public Vector2 ReturnInputDirection ();
    public InputActionPhase ReturnInputState ();
    public bool FireButtonPerformed ();
}
