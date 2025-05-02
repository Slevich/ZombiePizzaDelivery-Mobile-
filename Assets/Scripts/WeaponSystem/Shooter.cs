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
    [Header("Spawned bullet parent storage."), SerializeField] private ObservableObjectsStorage _storage;
    [Header("Event on single shot."), SerializeField] private UnityEvent _additionalOnShot;

    private bool _shootingInProcess = false;
    private List<ShotInfo> _shotInfos = new List<ShotInfo>();
    private ActionInterval _currentInterval;

    private static readonly Action<ShotInfo, ObservableObjectsStorage> _onShotInterval = (shot, storage) =>
    {
        Vector3 shotDirection = shot.FirePoint.ReturnWorldAxisDirection();
        GameObject bulletPrefab = shot.WeaponInfo.Info.Bullet;
        GameObject bulletClone = ObjectsSpawner.SpawnWithPositionRotation(bulletPrefab, shot.FirePoint.transform.position, Quaternion.identity);

        DirectiveMovement movement = ComponentsSearcher.GetComponentFromObject<DirectiveMovement>(bulletClone);

        if (movement == null)
            movement = bulletClone.AddComponent<DirectiveMovement>();

        movement.Direction = shotDirection;
        movement.Speed = shot.WeaponInfo.Info.BulletSpeed;
        shot.WeaponInfo.OnShot?.Invoke();
        storage.AddObjectToStorage(bulletClone, true);
        Debug.Log("FIRE!");
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
        Component[] findingComponents = ComponentsSearcher.GetComponentsFromObjectAndAllChildren(Weapon, new[] { typeof(WeaponInfoContainer), typeof(DirectionPoint)  } );

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
            firingAction += delegate { _onShotInterval?.Invoke(shot, _storage); } ;
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
