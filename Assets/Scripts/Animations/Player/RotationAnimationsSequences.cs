using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ModestTree;
using UnityEditor;
using UnityEngine;

public class RotationAnimationsSequences : MonoBehaviour
{
    [SerializeField] List<BasedOnFloatDirectionAnimationsSequence> _sequences = new List<BasedOnFloatDirectionAnimationsSequence>();
    private AxisMover _mover;
    private BasedOnFloatDirectionAnimationsSequence _currentSequence;
    private Direction _currentDirection = Direction.Zero;

    private bool _moverIsNull => _mover == null;
    private bool _isPlaying => _currentSequence != null;

    private void Awake()
    {
        _mover = PlayerReferencesContainer.Instance.Mover;

        if(_moverIsNull)
        {
            Debug.LogWarning("Mover reference is null!");
            return;
        }

        if (_sequences == null || _sequences.Count == 0)
        {
            Debug.LogError("List of sequences is empty!");
            return;
        }
    }

    private void DetectChangingDirection(float InputDirection)
    {
        Direction direction = Direction.Zero;

        if (InputDirection < 0)
            direction = Direction.Negative;
        else if (InputDirection == 0)
            direction = Direction.Zero;
        else if (InputDirection > 0)
            direction = Direction.Positive;

        if(_currentDirection != direction)
        {
            _currentDirection = direction;
            BasedOnFloatDirectionAnimationsSequence sequenceWithType = _sequences.Where(sequence => sequence.SequenceDirection == _currentDirection).FirstOrDefault();

            if(sequenceWithType != null)
            {
                if (_currentSequence != null)
                {
                    _currentSequence.Kill();
                }

                _currentSequence = sequenceWithType;
                _currentSequence.Start(delegate { _currentSequence = null; });
            }
        }
    }

    private void OnEnable ()
    {
        if(!_moverIsNull)
            _mover.OnMovementDirection += (direction) => DetectChangingDirection(direction);
    }

    private void OnDisable ()
    {
        if (_currentSequence != null && _isPlaying)
            _currentSequence.Kill();

        if (!_moverIsNull)
            _mover.OnMovementDirection += (direction) => DetectChangingDirection(direction);
    }
}

[Serializable]
public class BasedOnFloatDirectionAnimationsSequence
{
    [field: SerializeField] public Direction SequenceDirection { get; set; } = Direction.Zero;
    [SerializeField] private List<RotationAnimation> _animations;

    private bool _sequenceEmpty = false;
    private Sequence _animationsSequence;

    public void FillSequence(Action OnEndCallback)
    {
        if(_animations == null || _animations.Count == 0)
        {
            _sequenceEmpty = true;
            Debug.Log("Sequence is empty!");
            return;
        }

        _animationsSequence = DOTween.Sequence();
        RotationAnimation firstAnimation = _animations.First();
        Tween firstTween = firstAnimation.TweenAnimation;
        _animationsSequence.Append(firstTween);

        for (int i = 1; i < _animations.Count; i++)
        {
            Tween sequenceTween = _animations[i].TweenAnimation;
            _animationsSequence.Join(sequenceTween);
        }

        _animationsSequence.onComplete += delegate { OnEndCallback?.Invoke(); /*Debug.Log("Завершилась!");*/ };
    }

    public void Start(Action OnEndCallback)
    {
        if (_sequenceEmpty)
            return;

        FillSequence(OnEndCallback);
        _animationsSequence.Play();
    }

    public void Kill()
    {
        if(_animationsSequence.IsPlaying())
        {
            _animationsSequence.Kill(true);
        }
    }
}

public enum Direction
{
    Positive,
    Negative,
    Zero
}

[Serializable]
public class RotationAnimation
{
    //[SerializeField, ReadOnly] private string _name = string.Empty;
    [Header("Transform to rotate."), SerializeField] private Transform _animatedTransform;
    [Header("Collection of target Vector3 values."), SerializeField] private Vector3 _targetRotation = Vector3.zero;
    [Header("Time length of the animation."), SerializeField] private float _rotationDuration = 1f;
    [Header("Local or global rotation."), SerializeField] private TransformType _rotationType = TransformType.Local;
    [Header("Ease type."), SerializeField] private Ease _rotationEase = Ease.Linear;

    public Tween TweenAnimation 
    { 
        get
        {
            if (_rotationType == TransformType.Local)
            {
                return TweenTransformAnimationLibrary.RotateToLocalTarget(_animatedTransform, _targetRotation, _rotationDuration, _rotationEase);
            }
            else
                return TweenTransformAnimationLibrary.RotateToLocalTarget(_animatedTransform, _targetRotation, _rotationDuration, _rotationEase);
        }
    }
    public Transform AnimatedTransform => _animatedTransform;

    public bool TransformIsNull() => _animatedTransform == null;
}

public enum TransformType
{
    Local,
    Global
}

//[CustomPropertyDrawer(typeof(RotationAnimation))]
//public class RotationAnimationDrawer : PropertyDrawer
//{
//    private Dictionary<string, SerializedProperty> _serializedProperties = new Dictionary<string, SerializedProperty>()
//    {
//        {"_name", null},
//        {"_animatedTransform", null},
//        {"_targetRotations", null},
//        {"_rotationDuration", null},
//    };

//    private void FindAndGetProperties(SerializedProperty parentProperty)
//    {
//        string[] names = _serializedProperties.Keys.Select(key => key).ToArray();

//        foreach(string key in names)
//        {
//            _serializedProperties[key] = parentProperty.FindPropertyRelative(key);
//        }
//    }

//    //public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
//    //{
//    //    //property.serializedObject.Update();
//    //    //FindAndGetProperties(property);

//    //    //foreach(KeyValuePair<string, SerializedProperty> pair in _serializedProperties)
//    //    //{
//    //    //    EditorGUILayout.PropertyField(pair.Value);
//    //    //}

//    //    //property.serializedObject.ApplyModifiedProperties();
//    //    base.OnGUI (position, property, label);
//    //}
//}