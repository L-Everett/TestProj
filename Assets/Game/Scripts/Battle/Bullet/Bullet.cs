using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Animator mBulletAnimator;
    private float mDirection;
    private float mMoveSpeed;
    private bool mInitOver = false;

    /// <summary>
    /// 初始化子弹
    /// </summary>
    /// <param name="direction">移动方向 1 -> 右, -1 -> 左</param>
    /// <param name="moveSpeed">移动速度</param>
    public void Init(float direction, float moveSpeed)
    {
        mDirection = direction;
        mMoveSpeed = moveSpeed;
        mInitOver = true;
    }

    public void SetBullet()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        if (!mInitOver) return;
        transform.position += new Vector3(mDirection * mMoveSpeed * Time.deltaTime, 0, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {

        }
        else if (collision.collider.CompareTag("Wall"))
        {

        }
        else if (collision.collider.CompareTag("Enemy"))
        {

        }
        SetBoomAnim();
    }

    void SetBoomAnim()
    {
        mBulletAnimator.SetTrigger("Boom");
    }
}
