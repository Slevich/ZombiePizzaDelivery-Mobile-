using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

public class ObjectsDestroyer : MonoBehaviour
{
    #region Fields
    [Header("Destroy object itsel or other objects from collection.")]
    [SerializeField]
    private DestroyType _destroyType = DestroyType.Self;
    [Header("Gameobjects to destroy.")]
    [SerializeField] 
    private GameObject[] _destroyedObjects = new GameObject[] { };

    private Action<GameObject> _destroyAction;
    #endregion

    #region Methods
    private void Destroy (Action<GameObject> destroyAction)
    {
        switch(_destroyType)
        {
            case DestroyType.Self:
                destroyAction(gameObject);
                break;

            case DestroyType.Others:
                bool containsCurrentObject = _destroyedObjects.Where(gameObject => gameObject == this.gameObject).First();

                foreach(GameObject obj in _destroyedObjects)
                {
                    if (obj == this.gameObject)
                        continue;

                    destroyAction(obj);
                }

                if(containsCurrentObject)
                    destroyAction(gameObject);
                break;
        }
    }

    public void DestroyInRuntime()
    {
        _destroyAction = (gameObject => UnityEngine.Object.Destroy(gameObject));
        Destroy(_destroyAction);
    }

    public void DestroyImmediately()
    {
        _destroyAction = (gameObject => UnityEngine.Object.DestroyImmediate(gameObject));
        Destroy(_destroyAction);
    }
    #endregion
}

public enum DestroyType
{ 
    Self,
    Others
}

[CustomEditor(typeof(ObjectsDestroyer))]
public class ObjectsDestroyerEditor : Editor
{
    private static readonly string _destroyTypePropertyName = "_destroyType";
    private static readonly string _destroyObjectsPropertyName = "_destroyedObjects";

    private SerializedProperty _destroyTypeProperty;
    private SerializedProperty _destroyObjectsProperty;

    private void OnEnable ()
    {
        _destroyTypeProperty = serializedObject.FindProperty(_destroyTypePropertyName);
        _destroyObjectsProperty = serializedObject.FindProperty(_destroyObjectsPropertyName);
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_destroyTypeProperty);
        ObjectsDestroyer destroyer = target as ObjectsDestroyer;
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo destroyTypeField = destroyer.GetType().GetField(_destroyTypePropertyName, flags);
        DestroyType destroyType = (DestroyType)destroyTypeField.GetValue(destroyer);

        if (destroyType == DestroyType.Others)
            EditorGUILayout.PropertyField(_destroyObjectsProperty);

        serializedObject.ApplyModifiedProperties();
    }
}