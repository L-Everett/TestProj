using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("数值信息")]
    public float mMoveSpeed;

    [Header("组件")]
    public Rigidbody2D mRigidbody2D;
    public Collider2D mCollider2D;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }

    #region 移动相关
    //角色移动
    void PlayerMove()
    {
        float dir = Input.GetAxis("Horizontal");
        transform.position += new Vector3(Time.deltaTime * mMoveSpeed * dir, 0, 0);
        float lookAt = dir > 0 ? 1 : -1;
        transform.localScale = new Vector3(lookAt, transform.localScale.y, transform.localScale.z);
    }
    //镜头跟随
    void CameraFollow()
    {
        Camera mainCamera = Camera.main;

    }
    #endregion
}
