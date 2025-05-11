using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    #region Fields
    private Queue<DamageData> _currentDatas = new Queue<DamageData>();
    private ActionInterval _interval;
    private readonly int _maxQueue = 25;
    private readonly float _damageDesributionTime = 0.1f;
    #endregion

    #region Methods
    private void Awake ()
    {
        _interval = new ActionInterval();
    }

    public void AddNewData (DamageData NewData)
    {
        _currentDatas.Enqueue(NewData);

        if (_currentDatas.Count > 0 && !_interval.Busy)
            _interval.StartInterval(_damageDesributionTime, DamageDestribution);
    }

    private void DamageDestribution()
    {
        for(int i = 0; i <= _maxQueue || i < _currentDatas.Count; i++)
        {
            if (_currentDatas.Count == 0)
                break;

            DamageData data = _currentDatas.Dequeue();
            IDamageable damageTarget = data.Target;

            if(damageTarget.RequestToDamage())
                data.Target.CauseDamage(data.Amount);

            Destroy(data.Source.Parent.gameObject);
        }

        if(_currentDatas.Count == 0 && _interval.Busy)
        {
            _interval.Stop();
        }
    }
    #endregion
}
