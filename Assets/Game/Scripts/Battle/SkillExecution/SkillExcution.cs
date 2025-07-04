using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
[Skill("默认空技能模板", 2, typeof(SkillExcution))]
public class SkillConfig_Common : SkillConfigBase
{
    public override string InfoLabel { get; set; } = "默认空技能模板";
    [LabelText("执行器存在时间")]
    public float mActiveTimer;
}
public enum BaseSkillState
{
    BEHIND_SHAKE = 1,
    EXECUTE = 2,
    AFTER_SHAKE = 3,
}
public class SkillExcution:BaseExecution
{
    public SkillConfigBase mSkillConfig;
    public BaseSkillState mBattleBaseSkillState = BaseSkillState.BEHIND_SHAKE;

    public SkillData mSkillData;
    public float mSkillTimer = 0;


    public float mBeforeTimer=0;
    public float mAfterTimer=0;
    public void InitByConfig(SkillConfigBase config) //初始化
    {
        mSkillConfig = config;
        Init();
    }
    public override void Init()
    {
        base.Init();
    }
    public override void StartExecution()
    {
        base.StartExecution();
        mBattleBaseSkillState = BaseSkillState.BEHIND_SHAKE;
        mSkillTimer = 0;
        mBeforeTimer = 0;
        mAfterTimer = 0;
    }

    public override void BattleUpdate(float time)
    {
        if (IsFinish) //已经完成了，等待销毁，不用做处理
        {
            return;
        }

        base.BattleUpdate(time);
        if (mBattleBaseSkillState == BaseSkillState.BEHIND_SHAKE)
        {
            mBattleBaseSkillState = BaseSkillState.EXECUTE;
        }
        else if (mBattleBaseSkillState == BaseSkillState.EXECUTE)
        {
            ExecuteUpdate(time);
        }
        else if (mBattleBaseSkillState == BaseSkillState.AFTER_SHAKE)
        {
            FinishExecution();
        }
    }
    public virtual void ExecuteUpdate(float time) //更新处理
    {
        mSkillTimer = mSkillTimer + time;

        var config = mSkillConfig as SkillConfig_Common;
        if (mSkillTimer >= config.mActiveTimer)
        {
            mBattleBaseSkillState = BaseSkillState.AFTER_SHAKE;
        }
    }
    public override void FinishExecution()
    {
        if (GetParameter("finishedCallBack") != null)
        {
            var callBack = (Action)GetParameter("finishedCallBack");
            callBack?.Invoke();
        }
        base.FinishExecution();
    }
    public override void BreakExecution()
    {
        base.BreakExecution();
    }
}