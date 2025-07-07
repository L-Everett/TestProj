using UnityEngine;

public class BossMirrorAI : EnemyAI
{
    [Label("限制区-左")] public Transform mLeftLimit;
    [Label("限制区-右")] public Transform mRightLimit;

    private BossMirrorCtrl mCtrl;
    private Transform mPlayer;

    [Label("发呆时间")] public float mIdleTime;
    [Label("单轮最多攻击次数")] public int mMaxAttackCount;

    private float mIdleCoolTimer;  //发呆冷却计时器
    private int mAttackCount;  //攻击次数计数器

    void InitData()
    {
        mCtrl = mEnemyCtrl as BossMirrorCtrl;
        mPlayer = GameObject.Find("PlayerRoot").transform.GetChild(0);
        mIdleCoolTimer = mIdleTime;
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
            mCtrl.SetMoveTarget(mPlayer.position.x);
            mState = State.Attack;
        }
        // 巡逻
        else
        {
            if (transform.position.x <= mLeftLimit.position.x)
            {
                mCtrl.SetMoveTarget(mRightLimit.position.x);
            }
            else if (transform.position.x >= mRightLimit.position.x)
            {
                mCtrl.SetMoveTarget(mLeftLimit.position.x);
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
            int backCode = -1;
            mCtrl.SetDir(mPlayer.position.x - transform.position.x > 0 ? 1 : -1);
            if (Vector2.Distance(transform.position, mPlayer.position) > 3f)
            {
                mCtrl.Attack(1, (x) =>
                {
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
            if (backCode == 0) mAttackCount++;

            if (mAttackCount >= mMaxAttackCount)
            {
                mAttackCount = 0;
                mState = State.Idle;
                return;
            }
            mState = State.Move;
        }
        else
        {
            mState = State.Move;
        }
    }

    public override void Die()
    {

    }

}
