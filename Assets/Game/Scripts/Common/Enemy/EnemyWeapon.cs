using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public EnemyCtrl mEnemyCtrl;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBlock") && CompareTag("EnemyWeapon"))
        {
            PlayerCtrl player = collision.GetComponent<PlayerWeapon>().mPlayerCtrl;
            gameObject.tag = "Untagged";
            player.SetBlockCoolTime();
            player.ShowBlockEffect();
            player.mIsBlockInvincible = true;
            mEnemyCtrl.ByBlock(player.mLookAt);
        }
        else if (collision.CompareTag("Player") && CompareTag("EnemyWeapon"))
        {
            if (collision.GetComponent<PlayerCtrl>().mIsBlock) return;
            gameObject.tag = "Untagged";
            collision.GetComponent<PlayerCtrl>().Hurt(mEnemyCtrl.mLookAt);
        }
    }
}
