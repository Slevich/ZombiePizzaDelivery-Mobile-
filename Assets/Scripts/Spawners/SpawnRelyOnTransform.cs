using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnRelyOnTransform : MonoBehaviour
{
    #region Fields
    [Header("Position reference of the offset.")]
    [SerializeField] private Transform _anchor;
    [Space(2)]
    [Header("Transform parent for the spawned object.")]
    [SerializeField] private Transform _parent;
    [Space(2)]
    [Header("Offset in the global axes relative to the anchor.")]
    [SerializeField] public Vector3 _offset = Vector3.zero;
    [Space(3)]
    [Header("Spawned object prefab")]
    [SerializeField] GameObject _prefab;
    #endregion

    #region Properties
    public GameObject SpawnedObject { private set; get; }
    #endregion

    #region Methods
    public void Spawn()
    {
        Vector3 position = CalculateSpawnPosition();
        SpawnedObject = ObjectsSpawner.SpawnWithPositionRotationIntoParent(_prefab, position, Quaternion.identity, _parent);
        SpawnedObject.transform.SetParent(_parent);
    }

    private Vector3 CalculateSpawnPosition () => _anchor.position + _offset;

    #endregion
}

[CustomEditor(typeof(SpawnRelyOnTransform))]
public class SpawnRelyOnTransformEditor : Editor
{
    private static readonly string _anchorPropertyName = "_anchor";
    private static readonly string _prefabPropertyName = "_prefab";
    private static readonly string _offsetPropertyName = "_offset";

    private static readonly string _exampleName = "<SpawnExample>";

    private SerializedProperty _anchorProperty;
    private SerializedProperty _prefabProperty;
    private SerializedProperty _offsetProperty;

    private GameObject _componentHolder;
    private GameObject[] _childs;

    private Action _buttonAction;

    private bool _hasAnchor => _anchorProperty.boxedValue != null;
    private bool _hasPrefab => _prefabProperty.boxedValue != null;
    private string _buttonName = string.Empty;
    

    private void OnEnable ()
    {
        _anchorProperty = serializedObject.FindProperty(_anchorPropertyName);
        _prefabProperty = serializedObject.FindProperty(_prefabPropertyName);
        _offsetProperty = serializedObject.FindProperty(_offsetPropertyName);
        _componentHolder = serializedObject.targetObject.GameObject();
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        SpawnRelyOnTransform targetInstance = target as SpawnRelyOnTransform;
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        GUILayout.Space(20);
        bool hasExample = DetectExample(out GameObject example);

        if (hasExample)
        {
            Vector3 offset = CalculateOffset(example.transform);
            _offsetProperty.vector3Value = offset;
            _buttonName = "Hide example...";
            _buttonAction = delegate { example.GetComponent<ObjectsDestroyer>().DestroyImmediately(); };
        }
        else
        {
            _buttonName = "Show example!";
            _buttonAction = SpawnExample;
        }

        if(_hasPrefab && _hasAnchor && !Application.IsPlaying(serializedObject.targetObject))
        {
            bool _buttonPressed = GUILayout.Button(_buttonName);

            if (_buttonPressed)
                _buttonAction();
        }
        else if(hasExample)
            DestroyImmediate(example);

        serializedObject.ApplyModifiedProperties();

    }

    private bool DetectExample(out GameObject example)
    {
        int childsCount = _componentHolder.transform.childCount;
        List<GameObject> childs = new List<GameObject>();
        
        for(int i = 0; i < childsCount; i++)
        {
            Transform child = _componentHolder.transform.GetChild(i);
            childs.Add(child.gameObject);
        }

        _childs = childs.ToArray();

        example = childs.Where(child => child.gameObject.name == _exampleName).FirstOrDefault();

        return example != null;
    }

    private void SpawnExample()
    {
        GameObject example = ObjectsSpawner.SpawnWithPositionRotationIntoParent(
                                                                                    _prefabProperty.boxedValue as GameObject,
                                                                                    _componentHolder.transform.position,
                                                                                    Quaternion.identity,
                                                                                    _componentHolder.transform
                                                                               );
        example.name = _exampleName;
        ObjectsDestroyer destroyer = example.AddComponent<ObjectsDestroyer>();
        ParentDetector detector = example.AddComponent<ParentDetector>();
        detector.OnParentChanged.AddListener(delegate { destroyer.DestroyImmediately(); });
        detector.StartDetection();
    }

    private Vector3 CalculateOffset(Transform exampleTransform)
    {
        Vector3 _anchorPosition = ((Transform)_anchorProperty.boxedValue).position;
        return exampleTransform.position - _anchorPosition;
    }
}
