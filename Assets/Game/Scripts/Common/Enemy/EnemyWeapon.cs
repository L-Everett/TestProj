using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public EnemyCtrl mEnemyCtrl;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && CompareTag("PlayerWeapon"))
        {
            collision.GetComponent<PlayerCtrl>().Hurt(mEnemyCtrl.mLookAt);
        }
        else if (collision.CompareTag("PlayerBlock"))
        {
            Debug.Log("弹反成功");
        }
    }
}
