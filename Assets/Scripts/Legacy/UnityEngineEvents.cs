using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEngineEvents : MonoBehaviour
{
    #region Fields
    [Header("Event on Awake."), SerializeField] private UnityEvent _onAwake;
    [Header("Event on Start."), SerializeField] private UnityEvent _onStart;
    [Header("Event on Enable."), SerializeField] private UnityEvent _onEnable;
    [Header("Event on Disable."), SerializeField] private UnityEvent _onDisable;
    #endregion

    #region Methods
    private void Awake () => _onAwake?.Invoke();
    private void Start () => _onStart?.Invoke();
    private void OnEnable () => _onEnable?.Invoke();
    private void OnDisable () => _onDisable?.Invoke();
    #endregion
}
