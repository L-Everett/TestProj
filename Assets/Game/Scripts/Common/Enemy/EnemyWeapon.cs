using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public EnemyCtrl mEnemyCtrl;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerCtrl>().Hurt(mEnemyCtrl.mLookAt);
        }
    }
}
