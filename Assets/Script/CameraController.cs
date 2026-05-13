// 控制相機視角：按住滑鼠左鍵旋轉視角，使用滾輪縮放

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("滑鼠旋轉速度")]
    public float mouseSensitivity = 100f;

    [Header("滾輪縮放速度")]
    public float zoomSpeed = 20f;

    [Header("最小視野，數字越小越放大")]
    public float minFOV = 30f;

    [Header("最大視野，數字越大越廣角")]
    public float maxFOV = 70f;

    private Camera cam;

    private float xRotation;
    private float yRotation;

    // 避免重新啟用時滑鼠暴衝
    private bool ignoreNextFrame = false;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();

        SyncRotationWithCamera();

        if (cam != null)
        {
            cam.fieldOfView =
                Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }

    void Update()
    {
        // 忽略重新啟用的第一幀
        if (ignoreNextFrame)
        {
            ignoreNextFrame = false;
            return;
        }

        // 左鍵旋轉
        if (Input.GetMouseButton(0))
        {
            float mouseX =
                Input.GetAxis("Mouse X") *
                mouseSensitivity *
                Time.deltaTime;

            float mouseY =
                Input.GetAxis("Mouse Y") *
                mouseSensitivity *
                Time.deltaTime;

            yRotation += mouseX;
            xRotation -= mouseY;

            // 限制上下視角
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            transform.rotation =
                Quaternion.Euler(xRotation, yRotation, 0f);
        }

        // 滾輪縮放
        if (cam != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > 0.001f)
            {
                cam.fieldOfView -= scroll * zoomSpeed;

                cam.fieldOfView =
                    Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
            }
        }
    }

    // 導覽系統用來同步角度
    public void SyncRotationWithCamera()
    {
        Vector3 angles = transform.rotation.eulerAngles;

        yRotation = angles.y;

        xRotation = angles.x;

        // 修正 Unity Euler 角度問題
        if (xRotation > 180f)
        {
            xRotation -= 360f;
        }

        // 忽略下一幀輸入
        ignoreNextFrame = true;
    }
}