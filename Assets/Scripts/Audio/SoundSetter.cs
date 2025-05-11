using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundSetter : MonoBehaviour
{
    #region Fields
    [Header("AudioSource to set clip."), SerializeField] private AudioSource _audioSource;

    private WeaponHoldersManager _holder;
    private AudioClip _currentClip;
    #endregion

    #region Properties
    private bool sourceIsNull => _audioSource == null;
    private bool clipIsNull => _currentClip == null;
    #endregion

    #region Methods
    private void Awake ()
    {
        if (sourceIsNull && TryGetComponent<AudioSource>(out AudioSource source))
            _audioSource = source;

        if (sourceIsNull)
            return;

        _holder = PlayerReferencesContainer.Instance.HoldersManager;
        _holder.SpawnHeldObjectsCallback += (weapons) => GetClipFromObjects(weapons);
    }

    private void GetClipFromObjects (GameObject[] weapons)
    {
        if(weapons == null || weapons.Length == 0)
        {
            return;
        }

        if (sourceIsNull)
        {
            _audioSource.clip = null;
            _currentClip = null;
            return;
        }

        GameObject firstWeapon = weapons.First();
        Component[] findingComponents = ComponentsSearcher.GetComponentsOfTypesFromObjectAndAllChildren(firstWeapon, new[] { typeof(WeaponInfoContainer), typeof(DirectionPoint) });
        WeaponInfoContainer weaponInfo = findingComponents.Where(comp => comp.GetType() == typeof(WeaponInfoContainer)).FirstOrDefault() as WeaponInfoContainer;
        _currentClip = weaponInfo.Info.ShootSound;
        _audioSource.clip = _currentClip;
    }
    #endregion
}
