using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class TestAsync : MonoBehaviour
{
    CancellationTokenSource cancellationTokenSource;
    Task _currentTask;
    float _fixedDeltaTime = 0;

    private void Awake ()
    {
        _fixedDeltaTime = Time.fixedDeltaTime;   
    }

    public void CancelAsync()
    {
        if(_currentTask != null && !_currentTask.IsCanceled)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }
    }

    public async void StartAsync ()
    {
        if (_currentTask != null && !_currentTask.IsCanceled)
            return;

        int miliseconds = Mathf.RoundToInt(_fixedDeltaTime * 1000);
        Debug.Log("Галя у нас начало :)");

        _currentTask = Task.Run(async delegate
        {
            while(!cancellationTokenSource.IsCancellationRequested)
            {
                Debug.Log("Выполнение...");
                await Task.Delay(miliseconds);
            }
        }, 
        cancellationTokenSource.Token)
        .ContinueWith(delegate { Debug.Log("Галя, у нас отмена ;(");});
        

        await _currentTask;
        _currentTask = null;
    }

    private void OnEnable ()
    {
        cancellationTokenSource = new CancellationTokenSource ();
    }

    private void OnDisable ()
    {
        if(!cancellationTokenSource.IsCancellationRequested)
            cancellationTokenSource.Cancel ();
    }
}

[CustomEditor(typeof(TestAsync))]
public class TestAsyncEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        TestAsync testAsync = (TestAsync)target;
        MethodInfo[] methods = testAsync.GetType().GetMethods(flags).ToArray();

        bool start = GUILayout.Button("START!");
        if (start)
        {
            MethodInfo startMethod = methods.Where(method => method.Name == "StartAsync").First();
            startMethod.Invoke(testAsync, null);
        }

        bool canceled = GUILayout.Button("CANCEL!");
        if (canceled)
        {
            MethodInfo cancelMethod = methods.Where(method => method.Name == "CancelAsync").First();
            cancelMethod.Invoke(testAsync, null);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
