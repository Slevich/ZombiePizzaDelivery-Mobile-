using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    private void Awake ()
    {
        //InputHandler.Initialize ();
        //InputHandler.InputInfoEvent.Where(touch => touch.TouchPerformed).Subscribe(action => Debug.Log("Пальчик!")).AddTo(this);
        //InputHandler.InputInfoEvent.Where(touch => touch.TouchPerformed).Subscribe(touch => Debug.Log($"Touch position: {touch.CurrentScreenPosition}")).AddTo(this);
        //InputHandler.InputInfoEvent.Where(touch => touch.StartScreenPosition != Vector2.zero).Subscribe(touch => Debug.Log($"Start touch position: {touch.StartScreenPosition}")).AddTo(this);
    }

    //private void Update ()
    //{
    //    InputHandler.Performance();
    //}

    private void OnEnable ()
    {
        InputHandler.EnableInput();
    }

    private void OnDisable ()
    {
        InputHandler.DisableInput();
    }
}
