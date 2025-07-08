using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Boss - 镜中我
/// 复刻了玩家能力的Boss
/// </summary>
public class BossMirrorCtrl : EnemyCtrl
{
    [Header("组件")]
    public Rigidbody2D mRigidbody2D;
    public Collider2D mCollider2D;
    public Animator mAnimator;
    public SpriteRenderer mSpriteRenderer;
    public GameObject mWeapon;

    [Header("UI")]
    public Transform mHpRoot;
    public GameObject mHp;

    [Header("数值信息")]
    [Label("血量")] public int mHpCount;
    [Label("移动速度")] public float mMoveSpeed;
    [Label("可连跳次数")] public float mJumpMaxCount;
    [Label("跳跃力度")] public float mJumpForce;
    [Label("跳跃冷却时间")] public float mJumpMaxCoolTime;
    [Label("冲刺力度")] public float mRushForce;
    [Label("冲刺冷却时间")] public float mRushMaxCoolTime;
    [Label("冲刺持续时间")] public float mRushKeepTime;
    [Label("近战攻击冷却时间")] public float mNearAttackMaxCoolTime;
    [Label("远程攻击冷却时间")] public float mFarAttackMaxCoolTime;
    [Label("弹反冷却时间")] public float mBlockCoolTime;
    [Label("受击冷却时间")] public float mHurtCoolTime;

    #region 变量数据模块
    //--------------------------------战斗--------------------------------
    /// <summary>
    /// 当前血量值
    /// </summary>
    private int mCurHpCount;
    private List<HpCtrl> mHpCtrlList;
    /// <summary>
    /// 受伤冷却时间
    /// </summary>
    private float mHurtCoolTimer;
    /// <summary>
    /// 是否死亡
    /// </summary>
    private bool mIsDeath;
    /// <summary>
    /// 是否处于无敌
    /// </summary>
    private bool mIsInvincible;
    //-------------------------------------------------------------------

    //--------------------------------移动--------------------------------
    /// <summary>
    /// 角色朝向 1 -> 右， -1 -> 左
    /// </summary>
    //public int mLookAt;
    /// <summary>
    /// 是否可移动
    /// </summary>
    private bool mCanMove;
    /// <summary>
    /// 移动目标点
    /// </summary>
    private float mTargetPos;
    //-------------------------------------------------------------------

    //--------------------------------跳跃--------------------------------
    /// <summary>
    /// 当前跳跃次数
    /// </summary>
    private int mCurJumpCount;
    /// <summary>
    /// 跳跃冷却计时器
    /// </summary>
    private float mJumpCoolTimer;
    //-------------------------------------------------------------------

    //--------------------------------冲刺--------------------------------
    /// <summary>
    /// 角色当前是否在冲刺
    /// </summary>
    private bool mIsRush;
    /// <summary>
    /// 角色是否在空中冲刺过
    /// </summary>
    private bool mHasRushOnSky;
    /// <summary>
    /// 冲刺冷却计时器
    /// </summary>
    private float mRushCoolTimer;
    /// <summary>
    /// 冲刺持续时间计时器
    /// </summary>
    private float mRushKeepTimer;
    //-------------------------------------------------------------------

    //--------------------------------攻击--------------------------------
    /// <summary>
    /// 近战攻击冷却计时器
    /// </summary>
    private float mNearAttackCoolTimer;
    /// <summary>
    /// 远程攻击冷却计时器
    /// </summary>
    private float mFarAttackCoolTimer;
    /// <summary>
    /// 正在攻击
    /// </summary>
    private bool mIsAttack;
    /// <summary>
    /// 子弹预制体
    /// </summary>
    [Label("子弹"), SerializeField] private GameObject mBullet;
    //-------------------------------------------------------------------

    //--------------------------------弹反--------------------------------
    /// <summary>
    /// 弹反冷却计时器
    /// </summary>
    private float mBlockCoolTimer;
    /// <summary>
    /// 正在弹反
    /// </summary>
    private bool mIsBlock;
    //-------------------------------------------------------------------

    //--------------------------------其他--------------------------------
    /// <summary>
    /// 是否在地面
    /// </summary>
    private bool mIsOnGround;
    /// <summary>
    /// 角色当前动画状态
    /// </summary>
    private PlayerAnimState mCurAnimState;
    /// <summary>
    /// 角色初始重力规模
    /// </summary>
    private float mInitGravityScale;
    /// <summary>
    /// 初始颜色
    /// </summary>
    private Color mInitColor;
    //-------------------------------------------------------------------
    #endregion

    #region 初始化数据
    void InitData()
    {
        //战斗
        mCurHpCount = mHpCount;
        mHpCtrlList = new List<HpCtrl>();
        mHurtCoolTimer = mHurtCoolTime;

        //移动
        mLookAt = 1;
        mCanMove = true;
        mTargetPos = transform.position.x;

        //跳跃
        mCurJumpCount = 0;
        mJumpCoolTimer = 0;

        //冲刺
        mIsRush = false;
        mHasRushOnSky = false;
        mRushCoolTimer = mRushKeepTime;
        mRushKeepTimer = 0;

        //攻击
        mNearAttackCoolTimer = mNearAttackMaxCoolTime;
        mFarAttackCoolTimer = mFarAttackMaxCoolTime;

        //弹反
        mBlockCoolTimer = mBlockCoolTime;

        //其它
        mInitGravityScale = mRigidbody2D.gravityScale;
        mIsOnGround = true;
        mCurAnimState = PlayerAnimState.Idle;
        mInitColor = mSpriteRenderer.color;
    }
    #endregion

    private void Start()
    {
        InitData();
        InitUI();
    }

    private void FixedUpdate()
    {
        if (!mIsDeath)
        {
            Move();
        }
    }

    void Update()
    {
        UpdateTimer();
        SetMoveAnim();
    }

    #region 控制相关
    /// <summary>
    /// 更新冷却
    /// </summary>
    void UpdateTimer()
    {
        UpdateRush();
        UpdateJump();
        UpdateAttack();
        UpdateBlock();
        UpdateHurt();
    }

    public void SetIdle(bool canMove)
    {
        if (mCanMove == canMove) return;
        SetCanMove(false);
        SetAnim(PlayerAnimState.Idle);
    }

    //移动
    void Move()
    {
        if (!mCanMove) return;
        if ((new Vector2(transform.position.x - mTargetPos, 0)).sqrMagnitude < 0.3f * 0.3f) return;
        if (mIsRush) return;

        int dir = mTargetPos - transform.position.x > 0 ? 1 : -1;
        mRigidbody2D.velocity = new Vector2(dir * mMoveSpeed, Mathf.Clamp(mRigidbody2D.velocity.y, -20, 20));
        SetDir(dir);
    }
    public void SetCanMove(bool canMove)
    {
        if(mCanMove ==  canMove) return;
        mCanMove = canMove;
    }
    public void SetMoveTarget(float x)
    {
        if (mTargetPos == x) return;
        mTargetPos = x;
    }
    public float GetMoveTarget()
    {
        return mTargetPos;
    }

    public void Jump()
    {
        if (mJumpCoolTimer < mJumpMaxCoolTime) return;
        if (!mIsRush)
        {
            if (mCurJumpCount >= mJumpMaxCount) return;
            switch (mCurJumpCount)
            {
                case 0:
                    //mRigidbody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
                    mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, 20);
                    break;
                case 1:
                    mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, 0);
                    //mRigidbody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
                    mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, 20);
                    break;
            }
            mCurJumpCount++;
            mJumpCoolTimer = 0f;
            SetJumpAnim();
        }
    }
    void UpdateJump()
    {
        mJumpCoolTimer += Time.deltaTime;
    }

    public void Rush(Action<int> callback = null)
    {
        if (mRushCoolTimer >= mRushMaxCoolTime && !mIsAttack && !mIsBlock)
        {
            if (mIsOnGround || !mHasRushOnSky)
            {
                mRigidbody2D.velocity = new Vector2(0, 0);
                //mRigidbody2D.AddForce(mLookAt * mRushForce * Vector2.right, ForceMode2D.Impulse);
                mRigidbody2D.velocity = new Vector2(mLookAt * 16, mRigidbody2D.velocity.y);
                mRushCoolTimer = -mRushKeepTime;
                mIsRush = true;
                mCanMove = false;
                mHasRushOnSky = !mIsOnGround;
                SetRushAnim();
                callback?.Invoke(0);
            }
        }


    }
    void UpdateRush()
    {
        mRushCoolTimer += Time.deltaTime;
        if (mIsRush)
        {
            mRushKeepTimer += Time.deltaTime;
            mRigidbody2D.gravityScale = 0;
            if (mRushKeepTimer >= mRushKeepTime)
            {
                mIsRush = false;
                mCanMove = true;
                mRushKeepTimer = 0;
                mRigidbody2D.gravityScale = mInitGravityScale;
            }
        }
    }

    /// <summary>
    /// 攻击 F-近战 Q-远程
    /// </summary>
    /// <param name="type">0 -> 近战, 1 -> 远程</param>
    public void Attack(int type, Action<int> callback = null)
    {
        if (type == 0 && mNearAttackCoolTimer >= mNearAttackMaxCoolTime && !mIsAttack && !mIsRush)
        {
            var seq = DOTween.Sequence();
            seq.Append(mSpriteRenderer.DOColor(new Color(1, 0, 0), 0.1f));
            seq.Append(mSpriteRenderer.DOColor(mInitColor, 0.1f));
            mNearAttackCoolTimer = -0.7f;
            StartCoroutine(Near());
            callback?.Invoke(0);
        }
        if (type == 1 && mFarAttackCoolTimer >= mFarAttackMaxCoolTime && !mIsAttack && !mIsRush)
        {
            SetFarAttackAnim();
            StartCoroutine(Shot());
            mFarAttackCoolTimer = -0.6f;
            callback?.Invoke(0);
        }
        if (mIsAttack)
        {
            BanMoveAnim();
        }
    }
    void UpdateAttack()
    {
        mNearAttackCoolTimer += Time.deltaTime;
        mFarAttackCoolTimer += Time.deltaTime;
    }
    IEnumerator Shot()
    {
        yield return new WaitForSeconds(0.3f);
        mRigidbody2D.velocity = new Vector2(mLookAt * -3f, mRigidbody2D.velocity.y);
        Transform bullet = GameObject.Instantiate(mBullet, transform.parent).transform;
        bullet.position = transform.position + new Vector3(mLookAt, -0.8f, 0);
        bullet.localScale = new Vector3(mLookAt * bullet.localScale.x, bullet.localScale.y, bullet.localScale.z);
        bullet.GetComponent<Bullet>().Init(mLookAt, mMoveSpeed * 1.5f, 1); //玩家攻击-0，敌人攻击-1
    }
    IEnumerator Near()
    {
        yield return new WaitForSeconds(0.2f);
        SetNearAttackAnim();
        mRigidbody2D.velocity = new Vector2(mLookAt * 5f, mRigidbody2D.velocity.y);
        yield return new WaitForSeconds(0.3f);
    }
    IEnumerator AttackOver()
    {
        yield return new WaitForSeconds(0.5f);
        mIsAttack = false;
        mCanMove = true;
    }

    public void Block()
    {
        if (mBlockCoolTimer >= mBlockCoolTime && !mIsAttack && !mIsRush)
        {
            mIsBlock = true;
            mRigidbody2D.velocity = new Vector2(0, mRigidbody2D.velocity.y);
            BanMoveAnim();
            SetBlockAnim();
            mBlockCoolTimer = -0.2f;
            StartCoroutine(BlockOver());
        }
    }
    IEnumerator BlockOver()
    {
        yield return new WaitForSeconds(0.2f);
        mCanMove = true;
        mIsBlock = false;
    }
    void UpdateBlock()
    {
        mBlockCoolTimer += Time.deltaTime;
    }

    #endregion

    #region 碰撞相关
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") && mCurJumpCount > 0)
        {
            mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, 0);
            mIsOnGround = true;

            // 重置跳跃
            mCurJumpCount = 0;
            mJumpCoolTimer = mJumpMaxCoolTime;
            SetJumpAnim();

            //重置空中冲刺次数
            mHasRushOnSky = false;
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            //mRigidbody2D.velocity = new Vector2(0, mRigidbody2D.velocity.y);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && mCurJumpCount > 0)
        {
            mIsOnGround = false;
        }
    }

    #endregion

    #region 动画相关
    void SetAnim(PlayerAnimState playerAnimState)
    {
        mCurAnimState = playerAnimState;
        mAnimator.SetInteger("State", (int)mCurAnimState);
    }

    void SetMoveAnim()
    {
        if (mRigidbody2D.velocity.x != 0 && mCanMove)
        {
            if (mCurAnimState == PlayerAnimState.Jump1 || mCurAnimState == PlayerAnimState.Jump2) return;
            if(mRigidbody2D.velocity.x > 3) mAnimator.SetFloat("MoveSpeed", Mathf.Abs(mRigidbody2D.velocity.x) * mMoveSpeed / 12);
            if (mCurAnimState != PlayerAnimState.Move && mIsOnGround)
            {
                SetAnim(PlayerAnimState.Move);
            }
        }
    }

    void SetStopMoveAnim()
    {
        if (!mIsOnGround) return;
        if (mCurAnimState > PlayerAnimState.Move) return;
        if (mRigidbody2D.velocity.x != 0) return;
        mCurAnimState = PlayerAnimState.Idle;
        mAnimator.SetInteger("State", (int)mCurAnimState);
    }

    void SetJumpAnim()
    {
        if (mIsOnGround)
        {
            if (mCurJumpCount == 1 && mCurAnimState != PlayerAnimState.Jump1)
            {
                SetAnim(PlayerAnimState.Jump1);
            }
            else
            {
                SetAnim(PlayerAnimState.Idle);
            }
        }
        else
        {
            if (mCurJumpCount == 2 && mCurAnimState != PlayerAnimState.Jump2)
            {
                SetAnim(PlayerAnimState.Jump2);
            }
        }
    }

    void SetRushAnim()
    {
        mAnimator.SetTrigger("Rush");
    }

    void SetNearAttackAnim()
    {
        mIsAttack = true;
        mAnimator.SetTrigger("NearAttack");
        StartCoroutine(AttackOver());
    }

    void SetFarAttackAnim()
    {
        mIsAttack = true;
        mAnimator.SetTrigger("FarAttack");
        StartCoroutine(AttackOver());
    }

    void SetBlockAnim()
    {
        mAnimator.SetTrigger("Block");
    }

    void SetHurtAnim()
    {
        mAnimator.SetTrigger("Hurt");
    }

    void SetDieAnim()
    {
        mAnimator.SetTrigger("Die");
    }

    //非移动动画，需要禁用移动
    void BanMoveAnim()
    {
        //mRigidbody2D.velocity = new Vector2(0, mRigidbody2D.velocity.y);
        mCanMove = false;
        if (mCurAnimState == PlayerAnimState.Move)
        {
            SetStopMoveAnim();
        }
    }

    #endregion

    #region 战斗相关
    /// <summary>
    /// 设置的方向
    /// </summary>
    /// <returns></returns>
    public void SetDir(int dir)
    {
        if (mLookAt == dir) return;
        mRigidbody2D.velocity = Vector3.zero;
        mLookAt = -mLookAt;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * mLookAt, transform.localScale.y, transform.localScale.z);
    }
    /// <summary>
    /// 受伤后无敌一段时间
    /// </summary>
    /// <param name="hurtDirection">受击方向 1 -> 右， -1 -> 左</param>
    public override void Hurt(int hurtDirection)
    {
        //无敌状态
        if (mIsRush || mIsInvincible) return;
        if (mHurtCoolTimer >= mHurtCoolTime)
        {
            mCanMove = false;
            mRigidbody2D.velocity = new Vector2(0, mRigidbody2D.velocity.y);
            SetHurtAnim();
            mRigidbody2D.velocity = new Vector2(hurtDirection * 3f, mRigidbody2D.velocity.y);
            var sequence = DOTween.Sequence();
            sequence.Append(mSpriteRenderer.DOColor(new Color(1, 0, 0), 0.1f));
            sequence.Append(mSpriteRenderer.DOColor(new Color(1, 1, 1), 0.1f));
            sequence.AppendCallback(() =>
            {
                mCanMove = true;
            });
            sequence.Append(mSpriteRenderer.DOColor(new Color(1, 0, 0, 0.3f), 0.1f).SetLoops(6, LoopType.Yoyo));
            sequence.AppendCallback(() =>
            {
                mSpriteRenderer.color = mInitColor;
            });

            mCurHpCount--;
            mHurtCoolTimer = 0;
            FreshHpUI();
            Die();
        }
    }
    void UpdateHurt()
    {
        mHurtCoolTimer += Time.deltaTime;
    }

    void Die()
    {
        if (mCurHpCount <= 0)
        {
            mRigidbody2D.velocity = new Vector2(0, 0);
            mIsDeath = true;
            SetDieAnim();
            BanMoveAnim();
            StartCoroutine(Destory());
        }
    }

    IEnumerator Destory()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    /// <summary>
    /// 设置武器判断的标签, 0 -> 冲刺， 1 -> 弹反
    /// </summary>
    /// <param name="tag"></param>
    public void SetWeaponTag(int tag)
    {
        switch (tag)
        {
            case 0:
                mWeapon.tag = "EnemyWeapon";
                break;
            case 1:
                mWeapon.tag = "EnemyBlock";
                break;
        }
    }

    /// <summary>
    /// 被弹反
    /// </summary>
    /// <param name="dir"></param>
    public override void ByBlock(int dir)
    {
        mRigidbody2D.velocity = new Vector2(0, mRigidbody2D.velocity.y);
        mRigidbody2D.velocity = new Vector2(dir * 3f, mRigidbody2D.velocity.y);
    }
    #endregion

    #region UI相关
    void InitUI()
    {
        for (int i = 0; i < mHpCount; i++)
        {
            HpCtrl hp = GameObject.Instantiate(mHp, mHpRoot).GetComponent<HpCtrl>();
            hp.SetHp(true);
            mHpCtrlList.Add(hp);
        }
    }

    void FreshHpUI()
    {
        for (int i = 0; i < mHpCount; i++)
        {
            if (i < mCurHpCount)
            {
                mHpCtrlList[i].SetHp(true);
            }
            else
            {
                mHpCtrlList[i].SetHp(false);
            }
        }
    }
    #endregion
}
