using System;
using System.Collections.Generic;
using UnityEngine;


public class BaseExecution //基础的执行器，主要执行一些动作和技能，纯数据型类，无实体
{

    public bool mIsFinish = false;
    private Dictionary<string, object> parameters = null;

    public Action mFinishExecutionCallback = null;


    public virtual void Init() //初始化
    {

    }

    public virtual void StartExecution() //开始执行
    {
        Debug.Log("StartExecution StartExecution StartExecution StartExecution");
    }

    public virtual void FinishExecution() //完成执行
    {
        IsFinish = true;
        mFinishExecutionCallback?.Invoke();
    }

    public virtual void BreakExecution() //中断执行
    {
        IsFinish = true;
    }

    public virtual void BattleUpdate(float time) //更新处理
    {

    }

    public object GetParameter(string key)
    {
        if (parameters == null || !parameters.ContainsKey(key)) return null;
        else return parameters[key];
    }

    public Dictionary<string, object> GetParameter()
    {
        return parameters;
    }

    public void SetParameter(Dictionary<string, object> parameters)
    {
        this.parameters = parameters;
    }

    public bool IsFinish
    {
        get { return mIsFinish; }
        set { mIsFinish = value; }
    }

    public Action FinishExecutionCallback
    {
        get { return mFinishExecutionCallback; }
        set { mFinishExecutionCallback = value; }
    }
}
