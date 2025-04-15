using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using static UnityEngine.Object;

public static class ObjectsSpawner
{
    #region Func
    private static Func<GameObject, GameObject> simpleSpawnFunc = (prefab)
        => Instantiate(prefab);

    private static Func<GameObject, Transform, GameObject> intoParentSpawnFunc = (prefab, parent)
        => Instantiate(prefab, parent);

    private static Func<GameObject, Transform, bool, GameObject> intoParentSpawnFuncWithWorldSpaceParamFunc = (prefab, parent, inWorldSpace)
        => Instantiate(prefab, parent, inWorldSpace);

    private static Func<GameObject, Vector3, Quaternion, GameObject> spawnWithPositionAndRotationFunc = (prefab, position, rotation)
        => Instantiate(prefab, position, rotation);

    private static Func<GameObject, Vector3, Quaternion, Transform, GameObject> intoParentSpawnWithPositionAndRotationFunc = (prefab, position, rotation, parent)
        => Instantiate(prefab, position, rotation, parent);
    #endregion

    #region Methods
    private static GameObject ReturnSpawnedObject(GameObject gameObject, IEnumerable<Type> Components = null)
    {
        if (Components == null)
            return gameObject;

        foreach (Type component in Components)
            gameObject.AddComponent(component);

        return gameObject;
    }

    /// <summary>
    /// Spawn in world space with null parent and return object.
    /// </summary>
    /// <param name="Prefab">GameObject prefab reference.</param>
    /// <param name="Components">Collection of monobehavior types (components).</param>
    /// <returns>Spawned new gameobject.</returns>
    public static GameObject Spawn (GameObject Prefab, IEnumerable<Type> Components = null)
        => ReturnSpawnedObject(simpleSpawnFunc(Prefab), Components);

    /// <summary>
    /// Spawn with transform parent and return object.
    /// </summary>
    /// <param name="Prefab">GameObject prefab reference.</param>
    /// <param name="Parent">Transform of the spawned gameobject parent.</param>
    /// <param name="Components">Collection of monobehavior types (components).</param>
    /// <returns>Spawned new gameobject.</returns>
    public static GameObject SpawnIntoParent(GameObject Prefab, Transform Parent, IEnumerable<Type> Components = null)
        => ReturnSpawnedObject(intoParentSpawnFunc(Prefab, Parent), Components);

    /// <summary>
    /// Spawn with transform parent, can leave in world space and return object.
    /// </summary>
    /// <param name="Prefab">GameObject prefab reference.</param>
    /// <param name="Parent">Transform of the spawned gameobject parent.</param>
    /// <param name="InstantiateInWorldSpace"></param>
    /// <param name="Components">Collection of monobehavior types (components).</param>
    /// <returns>Spawned new gameobject.</returns>
    public static GameObject SpawnIntoParentWorldSpace (GameObject Prefab, Transform Parent, bool InstantiateInWorldSpace, IEnumerable<Type> Components = null)
        => ReturnSpawnedObject(intoParentSpawnFuncWithWorldSpaceParamFunc(Prefab, Parent, InstantiateInWorldSpace), Components);

    /// <summary>
    /// Spawn with setted position and rotation in world space, without parent, returns object.
    /// </summary>
    /// <param name="Prefab">GameObject prefab reference.</param>
    /// <param name="Position">Object position in world space.</param>
    /// <param name="Rotation">Object rotation in world space.</param>
    /// <param name="Components">Collection of monobehavior types (components).</param>
    /// <returns>Spawned new gameobject.</returns>
    public static GameObject SpawnWithPositionRotation (GameObject Prefab, Vector3 Position, Quaternion Rotation, IEnumerable<Type> Components = null)
        => ReturnSpawnedObject(spawnWithPositionAndRotationFunc(Prefab, Position, Rotation), Components);

    /// <summary>
    /// Spawn with setted position and rotation in world space, with parent, returns object.
    /// </summary>
    /// <param name="Prefab">GameObject prefab reference.</param>
    /// <param name="Position">Object position in world space.</param>
    /// <param name="Rotation">Object rotation in world space.</param>
    /// <param name="Parent">Transform of the spawned gameobject parent.</param>
    /// <param name="Components">Collection of monobehavior types (components).</param>
    /// <returns>Spawned new gameobject.</returns>
    public static GameObject SpawnWithPositionRotationIntoParent (GameObject Prefab, Vector3 Position, Quaternion Rotation, Transform Parent, IEnumerable<Type> Components = null)
        =>ReturnSpawnedObject(intoParentSpawnWithPositionAndRotationFunc(Prefab, Position, Rotation, Parent), Components);
    #endregion
}
