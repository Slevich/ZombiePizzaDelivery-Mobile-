using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Shooter : MonoBehaviour
{
    #region Fields
    [Header("Weapon holder."), SerializeField] private WeaponHoldersManager _holder;
    [Header("Spawned bullet parent storage."), SerializeField] private ObservableObjectsStorage _bulletStorage;
    [Header("Spawned objects on bullet destroy."), SerializeField] private ObservableObjectsStorage _stuffStorage;
    [Header("Event on single shot."), SerializeField] private UnityEvent _additionalOnShot;

    private bool _shootingInProcess = false;
    private List<ShotInfo> _shotInfos = new List<ShotInfo>();
    private ActionInterval _currentInterval;

    private static readonly Action<ShotInfo, ObservableObjectsStorage, ObservableObjectsStorage> _onShotInterval = (shot, bulletStorage, stuffStorage) =>
    {
        Vector3 shotDirection = shot.FirePoint.ReturnWorldAxisDirection();
        GameObject bulletPrefab = shot.WeaponInfo.Info.Bullet;
        GameObject bulletClone = ObjectsSpawner.SpawnWithPositionRotation(bulletPrefab, shot.FirePoint.transform.position, Quaternion.identity);

        TriggerDamageDealer bulletDamage = (TriggerDamageDealer)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(bulletClone, typeof(TriggerDamageDealer)));
        if(bulletDamage == null)
            bulletDamage = bulletClone.AddComponent<TriggerDamageDealer>();

        bulletDamage.Damage = shot.WeaponInfo.Info.Damage;
        bulletDamage.OnCollideDamageable += (pos) =>
        {
            foreach (GameObject objectOnBulletDestroy in bulletDamage.ObjectToSpawnOnDestroy)
            {
                Debug.Log("Spawn!");
                GameObject bulletStuff = ObjectsSpawner.SpawnWithPositionRotation(objectOnBulletDestroy, pos, Quaternion.identity);
                stuffStorage.AddObjectToStorageWithParenting(bulletStuff);
            }
        };

        DirectiveMovement movement = (DirectiveMovement)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(bulletClone, typeof(DirectiveMovement)));
        if (movement == null)
            movement = bulletClone.AddComponent<DirectiveMovement>();

        movement.Direction = shotDirection;
        movement.Speed = shot.WeaponInfo.Info.BulletSpeed;

        OnObjectDestroy onDestroy = (OnObjectDestroy)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(bulletClone, typeof(OnObjectDestroy)));
        if (onDestroy == null)
            onDestroy = bulletClone.AddComponent<OnObjectDestroy>();

        bulletStorage.AddObjectToStorageWithParenting(bulletClone);
        shot.WeaponInfo.OnShot?.Invoke();
    };
    #endregion

    #region Properties
    private bool _holderIsNull => _holder == null;
    private bool _hasWeaponsToShot => _shotInfos != null && _shotInfos.Count > 0;
    #endregion

    private void Awake ()
    {
        if(_holderIsNull)
        {
            Debug.LogError($"Holder reference is null! --> {this.GetType().ToString()}");
            return;
        }

        _holder.SpawnHeldObjectsCallback += (spawnedWeapons) => GetWeaponsShotsInfos(spawnedWeapons);
    }

    private void GetWeaponsShotsInfos (GameObject[] weaponsToShoot)
    {
        _shotInfos.Clear();

        foreach(GameObject weapon in weaponsToShoot)
        {
            GetShotInfoFromWeaponObject(weapon);
        }
    }

    private void GetShotInfoFromWeaponObject(GameObject Weapon)
    {
        Component[] findingComponents = ComponentsSearcher.GetComponentsOfTypesFromObjectAndAllChildren(Weapon, new[] { typeof(WeaponInfoContainer), typeof(DirectionPoint)  } );

        WeaponInfoContainer weaponInfo = findingComponents.Where(comp => comp.GetType() == typeof(WeaponInfoContainer)).FirstOrDefault() as WeaponInfoContainer;

        if(weaponInfo == null)
        {
            Debug.LogError("Holding weapon doesn't contain info!");
            return;
        }

        DirectionPoint firePoint = findingComponents.Where(comp => comp.GetType() == typeof(DirectionPoint)).FirstOrDefault() as DirectionPoint;

        if (firePoint == null)
        {
            Debug.LogError("Holding weapon doesn't contain fire point!");
            return;
        }

        ShotInfo shotInfo = new ShotInfo(weaponInfo, firePoint);
        _shotInfos.Add(shotInfo);
    }

    public void StartShooting()
    {
        if (!_hasWeaponsToShot || _shootingInProcess)
            return;

        _shootingInProcess = true;

        Action firingAction = null;

        foreach (ShotInfo shot in _shotInfos)
        {
            firingAction += delegate { _onShotInterval?.Invoke(shot, _bulletStorage, _stuffStorage); } ;
        }

        firingAction += delegate { _additionalOnShot?.Invoke(); };

        FiringMode mode = (FiringMode)_shotInfos.Select(info => info.WeaponInfo.Info.Mode).FirstOrDefault();

        switch(mode)
        {
            case FiringMode.Single:
                firingAction?.Invoke();
                break;

            case FiringMode.Auto:
                float fireRate = (int)_shotInfos.Select(info => info.WeaponInfo.Info.FireRate).FirstOrDefault();
                float fireRatePerSecond = (float)fireRate / 60f;
                float secondsBetweenShots = 1f / fireRatePerSecond;

                if (_currentInterval == null)
                    _currentInterval = new ActionInterval();

                firingAction?.Invoke();
                _currentInterval.StartInterval(secondsBetweenShots, firingAction);
                break;
        }

    }

    public void StopShooting()
    {
        if(!_shootingInProcess)
            return;

        if(_currentInterval != null)
        {
            _currentInterval.Stop();
            _currentInterval = null;
        }

        _shootingInProcess = false;
    }

    private class ShotInfo
    {
        public WeaponInfoContainer WeaponInfo { get; private set; }
        public DirectionPoint FirePoint { get; private set; }

        public ShotInfo(WeaponInfoContainer weaponInfo, DirectionPoint firePoint)
        {
            WeaponInfo = weaponInfo;
            FirePoint = firePoint;
        }
    }
}
