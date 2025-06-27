using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随的物体")]
    public Transform mFollowTrans;
    [Header("地图边界")]
    public float mLeft;
    public float mRight;
    public float mTop;
    public float mBottom;

    private Camera mCamera;
    private float mCameraHalfWidth;
    private float mCameraHalfHeight;
    private Vector3 mCurrentVelocity;

    void Start()
    {
        mCamera = GetComponent<Camera>();
        mCameraHalfHeight = mCamera.orthographicSize;
        mCameraHalfWidth = mCameraHalfHeight * mCamera.aspect;
    }

    private void LateUpdate()
    {
        Vector3 targetPos = new Vector3(
            mFollowTrans.position.x,
            mFollowTrans.position.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref mCurrentVelocity, 0.5f);
        ApplyBoundaryRestrictions();
    }

    private void ApplyBoundaryRestrictions()
    {
        float clampedX = Mathf.Clamp(transform.position.x, mLeft + mCameraHalfWidth, mRight - mCameraHalfWidth);
        float clampedY = Mathf.Clamp(transform.position.y, mBottom + mCameraHalfHeight, mTop - mCameraHalfHeight);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
