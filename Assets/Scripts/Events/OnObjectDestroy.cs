using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OnObjectDestroy : MonoBehaviour
{
    public Action OnDestroyCallback { get; set; } = null;

    private void OnDestroy ()
    {
        OnDestroyCallback?.Invoke();

        if(OnDestroyCallback != null)
            OnDestroyCallback = null;
    }
}
