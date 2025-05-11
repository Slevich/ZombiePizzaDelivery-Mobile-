using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class MeshesColorsChangeAnimation : MonoBehaviour
{
    #region Fields
    [Header("Materials colors."), SerializeField] private Color[] _colors;
    [field: Header("Animation duration."), SerializeField, Range(0f, 10f)] public float Duration { get; set; } = 1f;

    private Sequence _currentSequence;
    private MeshRenderer[] _renderers;
    private Dictionary<Material, Color> _originalColors = new Dictionary<Material, Color> ();
    #endregion

    #region Properties
    #endregion

    #region Methods
    private void Awake()
    {
        _renderers = ComponentsSearcher.GetComponentsOfTypeFromObjectAndAllChildren(transform.gameObject, typeof(MeshRenderer)).Select(comp => comp as MeshRenderer).ToArray();
        foreach (MeshRenderer renderer in _renderers)
        {
            foreach (Material material in renderer.materials)
            {
                _originalColors.Add(material, material.color);
            }
        }

        _currentSequence = DOTween.Sequence();
    }        

    public void LerpToColorByIndex(int ColorIndex)
    {
        if (ColorIndex < 0 || ColorIndex >= _colors.Length)
        {
            Debug.LogError("Index of the color is out of range!");
            return;
        }

        Color targetColor = _colors[ColorIndex];
        _currentSequence = DOTween.Sequence();

        foreach (KeyValuePair<Material, Color> color in _originalColors)
        {
            _currentSequence.Join(color.Key.DOColor(targetColor, Duration));
        }
        
        _currentSequence.Play();
    }

    public void SetMaterialsToColorByIndex(int ColorIndex)
    {
        Color targetColor = _colors[ColorIndex];
        foreach (KeyValuePair<Material, Color> originalMaterial in _originalColors)
        {
            originalMaterial.Key.color = targetColor;
        }
    }

    public void LerpMaterialsToOriginal()
    {
        _currentSequence = DOTween.Sequence();

        foreach (KeyValuePair<Material, Color> color in _originalColors)
        {
            _currentSequence.Join(color.Key.DOColor(color.Value, Duration));
        }

        _currentSequence.Play();
    }

    public void SetMaterialsToOriginal()
    {
        foreach(KeyValuePair<Material, Color> originalMaterial in _originalColors)
        {
            originalMaterial.Key.color = originalMaterial.Value;
        }
    }

    private void OnDisable ()
    {
        if(_currentSequence != null && _currentSequence.IsPlaying())
            _currentSequence.Kill();

        SetMaterialsToOriginal();
    }
    #endregion
}
