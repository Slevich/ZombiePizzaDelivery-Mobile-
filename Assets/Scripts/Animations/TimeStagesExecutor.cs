using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using ModestTree;
using UnityEditor.SceneManagement;
using static Cinemachine.CinemachineCore;

public class TimeStagesExecutor : MonoBehaviour
{
    #region Fields
    [Header("Time stages (execution from 0 to last)."), SerializeField] private TimeStage[] _stages;
    [Header("Time duration."), SerializeField, Range(0f, 10f)] private float _duration = 0.5f;

    [SerializeField, HideInInspector] public int NumberOfStages => _stages.Length;
    private ActionInterval _interval;
    private static readonly float _stagesUpdateTime = 0.05f;
    private float _currentTime = 0f;
    private float _minTimeStep = 0.1f;
    #endregion

    #region Properties
    public TimeStage[] Stages => _stages;
    public float Durtion => _duration;
    #endregion

    #region Methods
    private void Awake ()
    {
        _interval = new ActionInterval();
    }

    [ExecuteInEditMode]
    public void UpdatePercentage (TimeStageEditorChange change)
    {
        if (NumberOfStages == 0)
            return;

        float totalTime = _stages.Select(stage => stage.Time).Sum();
        float stageTime = 0f;

        switch (change.Type)
        {
            case ChangeType.AddNewStage:

                if (NumberOfStages > 1)
                    stageTime = _stages[NumberOfStages - 2].Time + _minTimeStep;
                else
                    stageTime = _duration / 2;

                _stages.Last().Time = MathF.Round(stageTime, 2); ;

                break;

            case ChangeType.DeleteStage:

                float currentLastStageValue = _stages.Last().Time;

                if(currentLastStageValue >= _duration)
                {
                    _stages.Last().Time = MathF.Round(_duration - _minTimeStep);
                }

                break;

            case ChangeType.ChangeSingleStageValue:

                int index = change.IndexOfElement;
                UpdateSingleStage(_stages[index]);
                break;

            case ChangeType.ChangeDuration:

                if(_duration < _minTimeStep)
                    _duration = _minTimeStep;

                foreach(TimeStage stage in _stages)
                {
                    UpdateSingleStage(stage);
                }

                break;
        }
    }

    private void UpdateSingleStage(TimeStage singleStage)
    {
        int index = _stages.IndexOf(singleStage);
        float stageCurrentTime = singleStage.Time;
        float minValue = 0;
        float maxValue = 0;
        float stageTime = singleStage.Time;

        if (NumberOfStages == 1)
        {
            minValue = 0;
            maxValue = _duration;
        }
        else if (index == NumberOfStages - 1)
        {
            minValue = _stages[index - 1].Time;
            maxValue = _duration;
        }
        else
        {
            minValue = _stages[index - 1].Time;
            maxValue = _stages[index + 1].Time;
        }

        stageTime = SetValueIntoBorders(stageCurrentTime, minValue, maxValue);
        singleStage.Time = MathF.Round(stageTime, 2);
    }

    private float SetValueIntoBorders (float checkedValue, float minValue, float maxValue)
    {
        if(checkedValue < minValue)
        {
            return minValue + _minTimeStep;
        }
        else if (checkedValue > maxValue)
        {
            return maxValue - _minTimeStep;
        }

        return checkedValue;
    }


    public void StartTime()
    {
        if(NumberOfStages == 0)
        {
            Debug.LogError($"Executor has no time stages! Object: {gameObject.name}!");
            return;
        }

        StopTime();

        Action onInterval = delegate
        {
            _currentTime += MathF.Round(_stagesUpdateTime, 2);

            if (_currentTime >= _duration)
            {
                StopTime();
                return;
            }

            int currentIndex = DefineCurrentStageIndex(_currentTime);
            TryToExecuteStage(currentIndex);
        };

        _interval.StartInterval(_stagesUpdateTime, onInterval);
    }

    public void StopTime()
    {
        if(_interval != null && _interval.Busy)
        {
            _currentTime = 0;
            _interval.Stop();
            ResetStages();
        }
    }

    private void TryToExecuteStage(int index)
    {
        if(index >= _stages.Length || index < 0)
            return;

        TimeStage currentStage = _stages[index];

        if(!currentStage.Executed)
            currentStage.Execute();
    }

    private int DefineCurrentStageIndex(float currentTime)
    {
        IEnumerable<TimeStage> leftStages = _stages.Where(stage => stage.Time > currentTime);

        if (leftStages == null || leftStages.Count() == 0)
            return -1;

        int index = _stages.IndexOf(leftStages.First());
        return index;
    }

    private void ResetStages()
    {
        foreach(TimeStage stage in _stages)
            stage.Executed = false;
    }

    private void OnDisable () => StopTime();
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(TimeStagesExecutor))]
public class TimeStagerExecutorEditor: Editor
{
    private SerializedProperty _stagesProperty;
    private SerializedProperty _durationProperty;

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();
        TimeStagesExecutor customizableAnimation = (TimeStagesExecutor)target;

        _stagesProperty = serializedObject.FindProperty("_stages");
        _durationProperty = serializedObject.FindProperty("_duration");


        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(_stagesProperty);
        EditorGUILayout.PropertyField(_durationProperty);

        TimeStageEditorChange change = null;

        if (EditorGUI.EndChangeCheck())
        {
            int stagesAmount = _stagesProperty.arraySize;

            if(stagesAmount != customizableAnimation.NumberOfStages)
            {
                ChangeType type = ChangeType.AddNewStage;

                if(stagesAmount < customizableAnimation.NumberOfStages)
                    type = ChangeType.DeleteStage;
                else if (stagesAmount > customizableAnimation.NumberOfStages)
                    type = ChangeType.AddNewStage;

                change = new TimeStageEditorChange(type);
            }
            else
            {
                if(_durationProperty.floatValue != customizableAnimation.Durtion)
                {
                    change = new TimeStageEditorChange(ChangeType.ChangeDuration);
                }
                else
                {
                    for (int i = 0; i < stagesAmount; i++)
                    {
                        SerializedProperty stageProperty = _stagesProperty.GetArrayElementAtIndex(i);
                        SerializedProperty percentageProperty = stageProperty.FindPropertyRelative("_time");
                        float boxedValue = percentageProperty.floatValue;

                        if (boxedValue != customizableAnimation.Stages[i].Time)
                        {
                            change = new TimeStageEditorChange(ChangeType.ChangeSingleStageValue, i, boxedValue);
                            break;
                        }
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();

        if(change != null)
        {
            customizableAnimation.UpdatePercentage(change);
        }
    }
}
#endif

public enum ChangeType
{
    AddNewStage,
    DeleteStage,
    ChangeSingleStageValue,
    ChangeDuration
}

public class TimeStageEditorChange
{
    public ChangeType Type { get; set; } = ChangeType.AddNewStage;
    public int IndexOfElement { get; set; } = -1;
    public float NewPercentageValue { get; set; } = 0f;

    public TimeStageEditorChange(ChangeType ChangeType, int Index = -1, float NewValue = 0f)
    {
        Type = ChangeType;
        IndexOfElement = Index;
        NewPercentageValue = NewValue;
    }
}

[Serializable]
public class TimeStage
{
    [Header("Stage time (between 0f and 1f)"), SerializeField] private float _time = 0.5f;
    [Header("Event invoked on changedStage."), SerializeField] private UnityEvent OnStage;
    [field: Header("Stage executed?"), SerializeField, ReadOnly] public bool Executed { get; set; } = false;
    public const float UpperRange = 1f;


    public float Time
    {
        get { return _time; }
        set { _time = value; }
    }

    public void Execute ()
    {
        if(Executed)
            return;

        OnStage?.Invoke();
        Executed = true;
    }
}