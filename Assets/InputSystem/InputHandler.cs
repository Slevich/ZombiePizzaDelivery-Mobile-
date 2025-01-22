using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;
using UniRx;

public static class InputHandler
{
    #region Fields
    private static InputActions _inputActions;

    public static Subject<IInputInfo[]> InputInfoEvent;
    //private static TouchInfo _touchInfo;
    private static ActionUpdate _update;

    private static Action _updateAction;

    private static IInputInfo[] _info;
    #endregion

    #region Properties
    static bool InputActionsIsNull
    {
        get
        {
            if (_inputActions != null)
                return false;
            else
            {
                Debug.LogError("Input actions is null!");
                return true;
            }
        }
    }
    #endregion

    #region Constructor
    static InputHandler()
    {
        Debug.Log("Инициализация инпута!");

        if(Application.isPlaying)
            Initialize();
    }
    #endregion

    #region Methods
    public static void Initialize()
    {
        _inputActions = new InputActions();
        InputInfoEvent = new Subject<IInputInfo[]>();
        _update = new ActionUpdate();

        _info = new IInputInfo[]
        {
            new TouchscreenInfo(_inputActions),
            new MouseKeyboardInfo(_inputActions)
        };

        _updateAction = delegate 
        {
            Debug.Log("Апдейт инпута!");
            InputInfoEvent.OnNext(_info);
        };
    }

    public static void EnableInput()
    {
        _inputActions.Enable();
        _update.StartUpdate(_updateAction);
        //_touchInfo.SubscribeActions();
    }

    public static void DisableInput()
    {
        _inputActions.Disable();
        _update.StopUpdate();
        //_touchInfo.DisposeActions();
    }
    #endregion
}