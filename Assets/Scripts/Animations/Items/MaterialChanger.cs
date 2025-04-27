using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialChanger : MonoBehaviour
{
    #region Fields
    [Header("Mesh renderer with the material."), SerializeField] private MeshRenderer _meshRenderer;
    [Header("Time in seconds to play animation."), SerializeField] private float _duration = 1f;
    [Header("Array of colors."),SerializeField] private Color[] _targetColors;

    private Material _flashMaterial;
    private Vector4 _emissionColor;
    private Tween _currentTween;
    private static readonly string _emissionPropertyName = "_EmissionColor";
    private static readonly float _maxIntensity = 4f;
    private static readonly float _emissionTimeStep = 0.01f;
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake ()
    {
        if (!_meshRenderer && TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
            _meshRenderer = renderer;

        if(_meshRenderer)
        {
            _flashMaterial = _meshRenderer.material;
            _emissionColor = _flashMaterial.GetVector(_emissionPropertyName);
        }
    }

    public void PlayColorLerpAnimation(int targerMaterialIndex)
    {
        if (_flashMaterial == null)
        {
            Debug.LogError("Material reference is null!");
            return;
        }

        if (targerMaterialIndex < 0 || targerMaterialIndex >= _targetColors.Length)
        {
            Debug.LogError("Index of the color is out of range!");
            return;
        }

        if (_currentTween != null && _currentTween.IsPlaying())
            _currentTween.Kill();


        Color endColor = _targetColors[targerMaterialIndex];
        _currentTween = _flashMaterial.DOColor(endColor, _duration);
        _currentTween.Play();
    }

    
    public void PlayColorFadeAnimation (float AlphaTargetValue)
    {
        if (_flashMaterial == null)
        {
            Debug.LogError("Material reference is null!");
            return;
        }

        if (AlphaTargetValue < 0 || AlphaTargetValue > 1)
        {
            Debug.LogError("Alpha target value must be between 0 and 1!");
            return;
        }

        if (_currentTween != null && _currentTween.IsPlaying())
            _currentTween.Kill();

        _currentTween = _flashMaterial.DOFade(AlphaTargetValue, _duration);
        _currentTween.Play();
    }


    public void PlayColorFlashAnimation(bool LerpEmission)
    {
        if (_flashMaterial == null)
        {
            Debug.LogError("Material reference is null!");
            return;
        }

        if (_currentTween != null && _currentTween.IsPlaying())
            _currentTween.Kill();

        SetAlphaValue(0);
        _currentTween = _flashMaterial.DOFade(1, _duration).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo);

        if (LerpEmission)
        {
            SetEmissionValue(0);
            float intensityStep = _maxIntensity * (_emissionTimeStep / _duration);
            float intensity = 0;
            bool moveBackwards = false;

            _currentTween.onStepComplete = delegate
            {
                moveBackwards = !moveBackwards;
            };

            _currentTween.onUpdate = delegate 
            {
                if (moveBackwards)
                    intensity -= intensityStep;
                else
                    intensity += intensityStep;

                //Debug.Log(intensity);
                SetEmissionValue(intensity);
            };

            _currentTween.onComplete = delegate
            {
                Debug.Log("Конец!");
                SetEmissionValue(0);
            };
        }
        _currentTween.Play();
    }

    public void SetAlphaValue(float AlphaValue)
    {
        if (_flashMaterial == null)
        {
            Debug.LogError("Material reference is null!");
            return;
        }

        if (AlphaValue < 0 || AlphaValue > 1)
        {
            Debug.LogError("Alpha target value must be between 0 and 1!");
            return;
        }

        Color color = _flashMaterial.color;
        color.a = AlphaValue;
        _flashMaterial.color = color;
    }

    public void SetEmissionValue (float Intensity)
    {
        if (_flashMaterial == null)
        {
            Debug.LogError("Material reference is null!");
            return;
        }

        _flashMaterial.SetVector(_emissionPropertyName, _emissionColor * Intensity);
    }

    private void OnDisable ()
    {
        if (_currentTween != null && _currentTween.IsPlaying())
            _currentTween.Kill();
    }
    #endregion
}
