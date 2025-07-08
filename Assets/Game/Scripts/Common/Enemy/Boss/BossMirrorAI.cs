using UnityEngine;

public class BossMirrorAI : EnemyAI
{
    [Label("限制区-左")] public Transform mLeftLimit;
    [Label("限制区-右")] public Transform mRightLimit;

    private BossMirrorCtrl mCtrl;
    private Transform mPlayer;

    [Label("发呆时间")] public float mIdleTime;
    [Label("单轮最多攻击次数")] public int mMaxAttackCount;
    [Label("指令间冷却时间")] public float mCommondCoolTime;

    private float mIdleCoolTimer;  //发呆冷却计时器
    private int mAttackCount;  //攻击次数计数器
    private float mCommondCoolTimer;

    void InitData()
    {
        mCtrl = mEnemyCtrl as BossMirrorCtrl;
        mPlayer = GameObject.Find("PlayerRoot").transform.GetChild(0);
        mIdleCoolTimer = mIdleTime;
        mCommondCoolTimer = mCommondCoolTime;
    }

    protected override void Start()
    {
        base.Start();
        InitData();
    }

    protected override void Update()
    {
        base.Update();
        UpdateCoolTimer();
    }

    // 检测玩家是否在区域
    bool CheckPlayer()
    {
        if(mPlayer != null)
        {
            if (mPlayer.position.x <= mRightLimit.position.x && mPlayer.position.x >= mLeftLimit.position.x)
            {
                return true;
            }
        }
        return false;
    }

    void UpdateCoolTimer()
    {
        mCommondCoolTimer += Time.deltaTime;
        mIdleCoolTimer += Time.deltaTime;
    }

    public override void Idle()
    {
        mCtrl.SetIdle(false);
        mIdleCoolTimer += Time.deltaTime;
        // Boss过了发呆时间
        if (mIdleCoolTimer >= mIdleTime)
        {
            if (CheckPlayer())
            {
                mState = State.Attack;
            }
            else
            {
                mCtrl.SetCanMove(true);
                mState = State.Move;
            }
            mIdleCoolTimer = 0;
        }
    }

    public override void Move()
    {
        if(CheckPlayer())
        {
            if(mCommondCoolTimer >= mCommondCoolTime)
            {
                float target = mPlayer.position.x - transform.position.x;
                target = target > 0 ? mPlayer.position.x - 2.5f : mPlayer.position.x + 2.5f;
                mCtrl.SetMoveTarget(target);
                mCtrl.SetCanMove(true);
                mState = State.Attack;
                mCommondCoolTimer = 0;
            }
        }
        // 巡逻
        else
        {
            if (transform.position.x <= mLeftLimit.position.x)
            {
                mCtrl.SetMoveTarget(mRightLimit.position.x);
                mCtrl.SetDir(1);
            }
            else if (transform.position.x >= mRightLimit.position.x)
            {
                mCtrl.SetMoveTarget(mLeftLimit.position.x);
                mCtrl.SetDir(-1);
            }
            else
            {
                if (mCtrl.mLookAt > 0)
                {
                    mCtrl.SetMoveTarget(mRightLimit.position.x);
                }
                else
                {
                    mCtrl.SetMoveTarget(mLeftLimit.position.x);
                }
            }
        }
    }

    public override void Attack()
    {
        if(CheckPlayer())
        {
            if (mCommondCoolTimer > mCommondCoolTime)
            {
                mCtrl.SetCanMove(false);
                int backCode = -1;
                mCtrl.SetDir(mPlayer.position.x - transform.position.x > 0 ? 1 : -1);
                float dis = Vector2.Distance(transform.position, mPlayer.position);
                if (dis > 5f)
                {
                    mCtrl.Attack(1, (x) =>
                    {
                        backCode = x;
                    });

                    if (backCode == -1) mCtrl.Rush((x) => {
                        backCode = x;
                    });
                }
                else
                {
                    mCtrl.Attack(0, (x) =>
                    {
                        backCode = x;
                    });
                }
                if (backCode == 0)
                {
                    mAttackCount++;
                    mCommondCoolTimer = 0;
                }

                if (mAttackCount >= mMaxAttackCount)
                {
                    mAttackCount = 0;
                    mState = State.Idle;
                    return;
                }
                mState = State.Move;
            }
        }
        else
        {
            mState = State.Move;
        }
    }

    public override void Defence()
    {
        mState = State.Idle;
    }

    public override void Die()
    {

    }

}
