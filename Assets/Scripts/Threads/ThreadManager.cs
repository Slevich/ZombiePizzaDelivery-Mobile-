using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System;

public static class ThreadManager
{
    #region Fields
    private static List<TrackingThread> _trackingThreads = new List<TrackingThread>();
    private static Thread _mainThread;
    private static CancellationTokenSource _cancellationTokenSource;
    #endregion

    #region Properties
    public static int TrackingThreandsCount => _trackingThreads.Count;
    #endregion

    #region Constructor
    static ThreadManager ()
    {
        if (!Application.isPlaying)
            return;

        _trackingThreads.Clear();
        _mainThread = Thread.CurrentThread;
        _cancellationTokenSource = new CancellationTokenSource();
        Application.quitting += delegate { _cancellationTokenSource.Cancel(); };
    }
    #endregion

    #region Methods
    public static bool TryToCreateThread(UnityAction ThreadAction, ThreadsMarks Mark = ThreadsMarks.Undefined, bool OnlyThisMark = true)
    {
        if (OnlyThisMark)
        {
            bool containsThreadsWithMark = _trackingThreads.Any(thread => thread.Mark == Mark);

            if (containsThreadsWithMark)
            {
                Debug.LogError($"Thread with mark {Mark} is already registered.");
                return false;
            }
        }

        bool registerSuccessful = RegisterThread(ThreadAction, Mark);

        if (registerSuccessful)
            return true;
        else
            return false;
    }

    private static bool RegisterThread(UnityAction ThreadAction, ThreadsMarks Mark = ThreadsMarks.Undefined)
    {
        TrackingThread newTrackingThread = new TrackingThread(ThreadAction, _cancellationTokenSource.Token, Mark, TrackingThreandsCount);

        ///ÑÄÅËÀÒÜ ÏĞÎÂÅĞÊÓ ÍÀ ÍÀËÈ×ÈÅ Â ËÈÑÒÅ ÒÎÃÎ ÆÅ ÏÎÒÎÊÀ.

        _trackingThreads.Add(newTrackingThread);
        return true;
    }

    public static string ReturnThreadsInfo()
    {
        if (TrackingThreandsCount == 0)
            return "There are no register threads.";

        string threadsInfo = string.Empty;

        for (int i = 0; i < TrackingThreandsCount; i++)
        {
            TrackingThread thread = _trackingThreads[i];
            threadsInfo += thread.ReturnThreadInfo();
        }

        return threadsInfo;
    }

    public static TrackingThread[] FindThreadsWithMark(ThreadsMarks Mark)
    {
        TrackingThread[] trackingThreadsWithMark = _trackingThreads.Where(thread => thread.Mark == Mark).ToArray();

        if (trackingThreadsWithMark == null || trackingThreadsWithMark.Length == 0)
            return new TrackingThread[] { };
        else
            return trackingThreadsWithMark;
    }

    public static bool ContainsThreadWithMark (ThreadsMarks Mark) => _trackingThreads.Any(thread => thread.Mark == Mark);
    #endregion
}

public class TrackingThread
{
    private Thread _thread;
    private CancellationToken _token;
    private UnityEvent _threadEvent = new UnityEvent();
    private List<UnityAction> _threadActions = new List<UnityAction>();
    private static readonly int _milisecondsRefreshRate = 100;

    public ThreadsMarks Mark { get; private set; }
    public int ID { get; private set; } = 0;
    public bool IsAlive => _thread.IsAlive;
    private bool hasListeners => _threadActions.Count > 0;

    public TrackingThread (UnityAction ThreadAction, CancellationToken Token, ThreadsMarks SettedMark = ThreadsMarks.Undefined, int SettedID = -1)
    {
        _token = Token;
        ThreadStart start = new ThreadStart(ThreadExecution);
        _thread = new Thread(start);
        Mark = SettedMark;

        if (SettedID == -1)
            ID = _thread.ManagedThreadId;
        else
            ID = SettedID;

        _thread.Name = $"{Mark}{ID}";
        _threadEvent = new UnityEvent();
        Subscribe(ThreadAction);
    }

    public void Start ()
    {
        if(!IsAlive)
            _thread.Start();
    }

    public void Abort ()
    {
        if (IsAlive)
            _thread.Abort();
    }

    public void Subscribe (UnityAction SubscribedAction)
    {
        if (_threadActions.Contains(SubscribedAction))
            return;

        _threadEvent.AddListener(SubscribedAction);
        _threadActions.Add(SubscribedAction);

        if(hasListeners)
            Start();
    }

    public void Dispose(UnityAction RemovedAction)
    {
        if(!_threadActions.Contains(RemovedAction))
            return;

        _threadEvent.RemoveListener(RemovedAction);
        _threadActions.Remove(RemovedAction);

        if(!hasListeners)
            Abort();
    }

    public string ReturnThreadInfo()
    {
        return $"Thread #{ID}\n" +
               $"Managed_ID: {_thread.ManagedThreadId}\n" +
               $"Is Alive: {_thread.IsAlive}\n" +
               $"Name: {_thread.Name}\n\n";
    }

    public bool IsTheSameThread(Thread EqualThread) => _thread == EqualThread;

    private async void ThreadExecution()
    {
        Task task = CallingEventAsync();
        await task;
    }

    private async Task CallingEventAsync()
    {
        await Task.Run(async delegate
        {
            do
            {
                _threadEvent?.Invoke();
                Debug.Log("Ïîòîê ïîøåë..");
                await Task.Delay(_milisecondsRefreshRate);
            }
            while (!_token.IsCancellationRequested);

            Abort();
        }, _token);
    }
}

public enum ThreadsMarks
{
    Animation,
    Spawn,
    Loading,
    Transformation,
    Undefined
}