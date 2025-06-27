using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("数值信息")]
    [Label("移动速度")] public float mMoveSpeed;
    [Label("可连跳次数")] public float mJumpMaxCount;
    [Label("跳跃力度")] public float mJumpForce;
    [Label("跳跃冷却时间")] public float mJumpMaxCoolTime;

    [Header("组件")]
    public Rigidbody2D mRigidbody2D;
    public Collider2D mCollider2D;
    public Animator mAnimator;

    private void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        Jump();
    }

    #region 控制相关
    //移动
    private float mLookAt = 1;
    void Move()
    {
        float dir = Input.GetAxisRaw("Horizontal");
        mRigidbody2D.velocity = new Vector2(dir * mMoveSpeed, Mathf.Clamp(mRigidbody2D.velocity.y, -20, 20));
        
        if (dir == 0 || mLookAt == dir) return;
        mLookAt = -mLookAt;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * mLookAt, transform.localScale.y, transform.localScale.z);
    }
    //跳跃
    private int mCurJumpCount = 0; //当前跳跃次数
    private float mJumpCoolTimer = 0; //跳跃冷却计时器
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

    #endregion

    #region 碰撞相关
    //落地检测
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground" && mCurJumpCount > 0)
        {
            mCurJumpCount = 0;
            mJumpCoolTimer = mJumpMaxCoolTime;
            mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, 0);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            mRigidbody2D.velocity = new Vector2(0, mRigidbody2D.velocity.y);
        }
    }
    #endregion

    #region 动画相关
    void SetMoveAnimation()
    {
        mAnimator.SetFloat("MoveVelocity", mRigidbody2D.velocity.x);
    }
    #endregion
}
