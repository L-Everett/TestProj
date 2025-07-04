using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public PlayerCtrl mPlayerCtrl;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyCtrl>().Hurt(mPlayerCtrl.mLookAt);
        }
    }
}
