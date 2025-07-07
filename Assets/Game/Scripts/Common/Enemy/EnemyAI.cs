using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Idle,
        Move, 
        Attack,
        Die
    }

    public State mState;
    public EnemyCtrl mEnemyCtrl;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        mState = State.Idle;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        switch (mState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Move:
                Move();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Die:
                Die();
                break;
        }
    }

    #region FSM
    public virtual void Idle()
    {

    }

    public virtual void Move()
    {

    }

    public virtual void Attack()
    {

    }

    public virtual void Die()
    {

    }
    #endregion
}
