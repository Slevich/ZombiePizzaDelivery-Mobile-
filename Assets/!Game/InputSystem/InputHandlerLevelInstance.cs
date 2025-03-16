using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerLevelInstance : MonoBehaviour
{
    private void OnEnable ()
    {
        InputHandler.EnableInput();
    }

    private void OnDisable ()
    {
        InputHandler.DisableInput();
    }
}
