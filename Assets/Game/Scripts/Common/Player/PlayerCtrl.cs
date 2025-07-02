using System.Collections;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("数值信息")]
    [Label("移动速度")] public float mMoveSpeed;
    [Label("可连跳次数")] public float mJumpMaxCount;
    [Label("跳跃力度")] public float mJumpForce;
    [Label("跳跃冷却时间")] public float mJumpMaxCoolTime;
    [Label("冲刺力度")] public float mRushForce;
    [Label("冲刺冷却时间")] public float mRushMaxCoolTime;
    [Label("冲刺持续时间")] public float mRushKeepTime;
    [Label("近战攻击冷却时间")] public float mNearAttackMaxCoolTime;
    [Label("远程攻击冷却时间")] public float mFarAttackMaxCoolTime;

    [Header("组件")]
    public Rigidbody2D mRigidbody2D;
    public Collider2D mCollider2D;
    public Animator mAnimator;

    #region 变量数据模块
    //--------------------------------移动--------------------------------
    /// <summary>
    /// 角色朝向 1 -> 右， -1 -> 左
    /// </summary>
    private float mLookAt;
    /// <summary>
    /// 是否可移动
    /// </summary>
    private bool mCanMove;
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
    [Label("子弹"), SerializeField]private GameObject mBullet;
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
    //-------------------------------------------------------------------
    #endregion

    #region 初始化数据
    void InitData()
    {
        //移动
        mLookAt = 1;
        mCanMove = true;

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

        //其它
        mInitGravityScale = mRigidbody2D.gravityScale;
        mIsOnGround = true;
        mCurAnimState = PlayerAnimState.Idle;
    }
    #endregion

    private void Start()
    {
        InitData();
    }

    private void FixedUpdate()
    {
        Move();

        SetMoveAnim();
        SetStopMoveAnim();
    }

    void Update()
    {
        Attack();
        if (!mIsAttack)
        {
            Jump();
            Rush();
        }

        SetJumpAnim();
    }

    #region 控制相关
    //移动
    void Move()
    {
        if (!mCanMove) return;
        float dir = Input.GetAxisRaw("Horizontal");
        mRigidbody2D.velocity = new Vector2(dir * mMoveSpeed, Mathf.Clamp(mRigidbody2D.velocity.y, -20, 20));

        if (dir == 0 || mLookAt == dir) return;
        mLookAt = -mLookAt;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * mLookAt, transform.localScale.y, transform.localScale.z);
    }

    //跳跃
    void Jump()
    {
        mJumpCoolTimer += Time.deltaTime;
        if (mJumpCoolTimer < mJumpMaxCoolTime) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mCurJumpCount >= mJumpMaxCount) return;
            switch (mCurJumpCount)
            {
                case 0:
                    mRigidbody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
                    break;
                case 1:
                    mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, 0);
                    mRigidbody2D.AddForce(Vector2.up * mJumpForce, ForceMode2D.Impulse);
                    break;
            }
            mCurJumpCount++;
            mJumpCoolTimer = 0f;
        }
    }

    // 鼠标左键冲刺
    void Rush()
    {
        mRushCoolTimer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && mRushCoolTimer >= mRushMaxCoolTime)
        {
            if (mIsOnGround || !mHasRushOnSky)
            {
                mRigidbody2D.velocity = new Vector2(0, 0);
                mRigidbody2D.AddForce(mLookAt * mRushForce * Vector2.right, ForceMode2D.Impulse);
                mRushCoolTimer = -mRushKeepTime;
                mIsRush = true;
                mCanMove = false;
                mHasRushOnSky = !mIsOnGround;
                SetRushAnim();
            }
        }
        
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

    //攻击 F-近战 Q-远程
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.F) && mNearAttackCoolTimer >= mNearAttackMaxCoolTime) 
        {
            SetNearAttackAnim();
        }
        if (Input.GetKeyDown(KeyCode.Q) && mFarAttackCoolTimer >= mFarAttackMaxCoolTime)
        {
            SetFarAttackAnim();
            StartCoroutine(Shot());
        }
        mNearAttackCoolTimer += Time.deltaTime;
        mFarAttackCoolTimer += Time.deltaTime;
    }
    IEnumerator Shot()
    {
        yield return new WaitForSeconds(0.2f);
        Transform bullet = GameObject.Instantiate(mBullet, transform.parent).transform;
        bullet.position = transform.position + new Vector3(mLookAt / 2, 0, 0);
        bullet.localScale = new Vector3(mLookAt * bullet.localScale.x, bullet.localScale.y, bullet.localScale.z);
        bullet.GetComponent<Bullet>().Init(mLookAt, mMoveSpeed * 1.5f);
    }
    IEnumerator AttackOver()
    {
        yield return new WaitForSeconds(0.3f);
        mIsAttack = false;
        mCanMove = true;
    }

    #endregion

    #region 碰撞相关
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Ground") && mCurJumpCount > 0)
        {
            // 重置跳跃
            mCurJumpCount = 0;
            mJumpCoolTimer = mJumpMaxCoolTime;

            mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, 0);
            mIsOnGround = true;

            //重置空中冲刺次数
            mHasRushOnSky = false;
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            mRigidbody2D.velocity = new Vector2(0, mRigidbody2D.velocity.y);
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
    void SetMoveAnim()
    {
        if(mRigidbody2D.velocity.x != 0 && mCanMove)
        {
            mAnimator.SetFloat("MoveSpeed", Mathf.Abs(mRigidbody2D.velocity.x) * mMoveSpeed / 16);
            if(mCurAnimState != PlayerAnimState.Move && mIsOnGround)
            {
                mCurAnimState = PlayerAnimState.Move;
                mAnimator.SetInteger("State", (int)mCurAnimState);
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
        if (mCurJumpCount == 1 && mCurAnimState != PlayerAnimState.Jump1)
        {
            mCurAnimState = PlayerAnimState.Jump1;
            mAnimator.SetInteger("State", (int)mCurAnimState);
        }
        else if (mCurJumpCount == 2 && mCurAnimState != PlayerAnimState.Jump2)
        {
            mCurAnimState = PlayerAnimState.Jump2;
            mAnimator.SetInteger("State", (int)mCurAnimState);
        }
        if (mIsOnGround)
        {
            if (mCurAnimState == PlayerAnimState.Jump1 || mCurAnimState == PlayerAnimState.Jump2)
            {
                mCurAnimState = PlayerAnimState.Idle;
                mAnimator.SetInteger("State", (int)mCurAnimState);
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
        mCanMove = false;
        mAnimator.SetTrigger("NearAttack");
        StartCoroutine(AttackOver());
    }

    void SetFarAttackAnim()
    {
        mIsAttack = true;
        mCanMove = false;
        mAnimator.SetTrigger("FarAttack");
        StartCoroutine(AttackOver());
    }

    #endregion
}
