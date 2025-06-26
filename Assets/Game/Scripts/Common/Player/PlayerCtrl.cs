using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("��ֵ��Ϣ")]
    public float mMoveSpeed;

    [Header("���")]
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

    #region �ƶ����
    //��ɫ�ƶ�
    void PlayerMove()
    {
        float dir = Input.GetAxis("Horizontal");
        transform.position += new Vector3(Time.deltaTime * mMoveSpeed * dir, 0, 0);
        float lookAt = dir > 0 ? 1 : -1;
        transform.localScale = new Vector3(lookAt, transform.localScale.y, transform.localScale.z);
    }
    //��ͷ����
    void CameraFollow()
    {
        Camera mainCamera = Camera.main;

    }
    #endregion
}
