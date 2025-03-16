using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLifetime : MonoBehaviour
{
    [field: Header("Object lifetime in seconds."), ReadOnly, SerializeField] public float LifeTime { get; set; } = 0f;

    public void IncreaseLifetime(float plusLifetime)
    {
        if(plusLifetime <= 0)
        {
            Debug.Log("Lifetime cannot increase by zero or negative value!");
            return;
        }

        LifeTime += plusLifetime;
    }
}
