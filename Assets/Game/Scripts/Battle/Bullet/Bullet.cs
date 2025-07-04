using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Animator mBulletAnimator;
    [SerializeField] private SpriteRenderer mSpriteRenderer;
    private int mDirection;
    private float mMoveSpeed;
    private bool mInitOver = false;
    private bool mCanMove = true;
    /// <summary>
    /// 施放者：0 -> 玩家，1 -> 敌人
    /// </summary>
    private int mReleaser;
    /// <summary>
    /// 可碰撞次数
    /// </summary>
    private int mCanCollision = 2;
    /// <summary>
    /// 是否被弹反
    /// </summary>
    private bool mIsBlock = false;
    /// <summary>
    /// 正在销毁
    /// </summary>
    private bool mIsDestory = false;

    /// <summary>
    /// 初始化子弹
    /// </summary>
    /// <param name="direction">移动方向 1 -> 右, -1 -> 左</param>
    /// <param name="moveSpeed">移动速度</param>
    /// <param name="releaser">施放者 0 -> 玩家，1 -> 敌人</param>
    public void Init(int direction, float moveSpeed, int releaser)
    {
        mDirection = direction;
        mMoveSpeed = moveSpeed;
        mInitOver = true;
        mReleaser = releaser;
        if (releaser == 1)
        {
            mSpriteRenderer.color = Color.red;
        }
    }

    /// <summary>
    /// 反弹子弹
    /// </summary>
    void SetBullet(int releaser)
    {
        mIsBlock = true;
        mDirection *= -1;
        mMoveSpeed *= 1.5f;
        mReleaser = releaser;
        mCanMove = true;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z) * 1.5f;
        if (releaser == 1)
        {
            mSpriteRenderer.color = Color.red;
        }
        else
        {
            mSpriteRenderer.color = new Color(1, 0, 232 / 255f);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!mInitOver) return;
        if (!mCanMove) return;
        transform.position += new Vector3(mDirection * mMoveSpeed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (mIsDestory) return;
        if (collision.CompareTag("Player"))
        {
            if (mReleaser == 0) return;
            collision.gameObject.GetComponent<PlayerCtrl>().Hurt(mDirection);
        }
        else if (collision.CompareTag("Enemy"))
        {
            if (mReleaser == 1) return;
            collision.gameObject.GetComponent<EnemyCtrl>().Hurt(mDirection);
        }
        else if (collision.CompareTag("PlayerBlock"))
        {
            if (mReleaser == 0) return;
            SetBullet(0);
            return;
        }
        else if (collision.CompareTag("PlayerWeapon"))
        {
            if (mReleaser == 0) return;
        }

        //被弹反的附带一次穿透效果
        if (mIsBlock)
        {
            mCanCollision--;
            if (mCanCollision != 0)
            {
                return;
            }
        }
        mCanMove = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        SetBoomAnim();
    }

    void SetBoomAnim()
    {
        mIsDestory = true;
        mBulletAnimator.SetTrigger("Boom");
        Destroy(gameObject, 0.5f);
    }

    private void OnDestroy()
    {
        
    }
}
