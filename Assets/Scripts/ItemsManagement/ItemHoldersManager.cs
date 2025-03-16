using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing;
using System.Linq;

public class ItemHoldersManager : MonoBehaviour
{
    #region Fields
    [Header("Start spawn points."), SerializeField] private HolderPoint[] _holders;
    [Header("Allowed item to grab."), SerializeField] private AllowedGrabbableItem[] _allowedItems;

    private GameObject _currentWeapon = null;
    //[Header("Original prefab."), SerializeField] private GameObject _prefabOriginal;
    //[Header("Parent for all spawned objects."), SerializeField] private Transform _spawnedObjectsParent;
    //[Header("Spawned object velocity modifier."), SerializeField] private float _speed = 0.1f;

    //private ObjectsLifetimeController _lifetimeController;
    //private MultipleObjectsMover _multipleObjectsMover;
    private float _fixedDeltaTime = 0f;
    #endregion

    #region Properties
    private bool _hasHolders
    {
        get
        {
            return _holders != null && _holders.Length > 0;
        }
    }
    #endregion

    #region Methods
    private void Awake ()
    {
        //_fixedDeltaTime = Time.fixedDeltaTime;
        //_lifetimeController = new ObjectsLifetimeController();
        //_multipleObjectsMover = new MultipleObjectsMover();
    }

    public void TryToGrabNewItem(ObtainableItem item)
    {
        if (_allowedItems.Length == 0)
            return;

        ObtainableWeapon weapon = item as ObtainableWeapon;
        GameObject obtainableWeapon = weapon.GrabbableWeaponItem;
        
        if(obtainableWeapon == _currentWeapon)
        {
            return;
        }

        _currentWeapon = obtainableWeapon;

        IEnumerable<AllowedGrabbableItem> allowedItemsMatches = _allowedItems.Where(item => item.HoldingItemPrefab == weapon.GrabbableWeaponItem);
        if (allowedItemsMatches == null || allowedItemsMatches.Count() == 0)
            return;

        Vector3 itemOffset = allowedItemsMatches.First().HolderOffset;
        Vector3 itemRotation = allowedItemsMatches.First().HolderRotation;

        foreach(HolderPoint holder in _holders)
        {
            holder.DestroyChildren();
            GameObject spawnedObject = Instantiate(_currentWeapon, holder.PointTransform, false);
            spawnedObject.transform.localPosition = itemOffset;
            spawnedObject.transform.localEulerAngles = itemRotation;
        }
    }

    //public void SpawnObjectEachPoint()
    //{
    //    if (!_hasHolders && _prefabOriginal != null)
    //        return;

    //    foreach(HolderPoint' point in _holders)
    //    {
    //        GameObject spawnedObject = Instantiate(_prefabOriginal, _spawnedObjectsParent, true);
    //        spawnedObject.transform.position = point.PointTransform.position;
    //        _lifetimeController.AddNewObservableObject(spawnedObject);
    //        _multipleObjectsMover.AddNewMovingObject(spawnedObject.transform, point.WorldDirection, _fixedDeltaTime * _speed);
    //    }
    //}

    //private void OnDisable ()
    //{
    //    _multipleObjectsMover.StopMovement();
    //    _lifetimeController.DestroyAllObjects();
    //}
    #endregion
}

[Serializable]
public class HolderPoint
{
    [field: Header("Transform."), SerializeField] public Transform PointTransform { get; set; }
    [field: Header("Assumed local axis of motion."), SerializeField] private Axes LocalDirectionAxis { get; set; } = Axes.X;

    private Vector3 _worldDirection = Vector3.zero;
    private Vector3 _localDirection = Vector3.zero;

    public Vector3 WorldDirection
    {
        get
        {
            if (_worldDirection == Vector3.zero)
                _worldDirection = CalculateWorldDirection();

            return _worldDirection;
        }
    }

    public Vector3 LocalDirection
    {
        get
        {
            if (_localDirection == Vector3.zero)
                _localDirection = ReturnLocalDirectionNormalized();

            return _localDirection;
        }
    }

    private Vector3 CalculateWorldDirection ()
    {
        Vector3 localDirection = LocalDirection;
        Vector3 worldDirection = PointTransform.parent != null ? PointTransform.parent.TransformDirection(localDirection) : localDirection;
        return worldDirection;
    }

    public Vector3 ReturnLocalDirectionNormalized ()
    {
        Vector3 localPosition = PointTransform.localPosition;
        Vector3 transformDirection = AxesSelector.ReturnVector(LocalDirectionAxis, (PointTransform.right, PointTransform.up, PointTransform.forward));
        Vector3 localTarget = localPosition + transformDirection;
        return VectorsOperations.CalculateDirection(localTarget, localPosition).normalized;
    }

    public void DestroyChildren()
    {
        int childCount = PointTransform.childCount;

        if (childCount == 0)
            return;

        for (int i = 0; i < childCount; i++)
        {
            GameObject childObject = PointTransform.GetChild(i).gameObject;
            MonoBehaviour.Destroy(childObject);
        }
    }
}

[Serializable]
public class AllowedGrabbableItem
{
    [field: Header("Link to prefab."), SerializeField] public GameObject HoldingItemPrefab { get; set; }
    [field: Header("Offset in holder local space."), SerializeField] public Vector3 HolderOffset { get; set; } = Vector3.zero;
    [field: Header("Rotation in holder local space."), SerializeField] public Vector3 HolderRotation { get; set; } = Vector3.zero;
}