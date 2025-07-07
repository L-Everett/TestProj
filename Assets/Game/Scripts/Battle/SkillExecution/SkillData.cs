using Engine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class SkillData
{
    public string mSkillId = "";
    public SkillDataObject mSkillData;
    public float mCooldownTime = 0f;
    public float mCooldownValue = 0f;

    public float mCheckRange = 0;


    public float mAwakeColdTime = 0;

    public bool mIsFinishInit = false;

    public void Init(string id,Dictionary<string,object> param=null) //初始化
    {
        mSkillId = id;


        //ResourceManager.LoadResourceAsync<SkillDataObject>($"Common/SkillDatas/Skill_{mSkillId}.asset", (skill) =>
        //{
        //    mSkillData = skill;


        //    mAwakeColdTime = mSkillData.AwakeColdTime;
        //    mCooldownTime = mSkillData.AwakeColdTime;
        //    mCooldownValue = mSkillData.ColdTime;
        //    mCheckRange = mSkillData.SkillCheckRange;
        //    mIsFinishInit = true;
        //});
    }

    public float GetSkillCheckRange()
    {
        float num = 0;

        num = mCheckRange;

        return num;
    }

    public float GetCurCooldownValue()
    {
        float num = mCooldownValue;
        return num;
    }


    public void ResetAwakeColdTime(float num = 0)
    {
        if (num != 0)
        {
            mCooldownTime = num;
        }
        else
        {
            mCooldownTime = mAwakeColdTime;
        }
    }

    public void BattleUpdate(float time) //更新处理
    {
        if (mSkillData == null) return;
        if (mCooldownTime > 0) mCooldownTime -= time;
        if (mCooldownTime < 0) mCooldownTime = 0;
    }


    public bool CanUse() //是否可以使用
    {
        bool key = true;
        if (mCooldownTime != 0)
        {
            key = false;
        }

        return key;
    }

    public void CoolDown() //是否可以使用
    {
        mCooldownTime = GetCurCooldownValue();
    }

    public BaseExecution CreateExecution()
    {
        Type type = mSkillData.SkillTemplateConfig.GetType().GetCustomAttribute<SkillAttribute>().ExcutionType;
        SkillExcution execution = Activator.CreateInstance(type) as SkillExcution;
        mSkillData.SkillTemplateConfig.Name = mSkillId;
        execution.InitByConfig(mSkillData.SkillTemplateConfig);
        execution.mSkillData = this;

        return execution;
    }
}

