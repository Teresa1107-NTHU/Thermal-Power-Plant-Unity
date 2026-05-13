using UnityEngine;
using UnityEngine.EventSystems;

public class TourCameraLook : MonoBehaviour
{
    [Header("旋轉速度")]
    public float rotationSpeed = 3f;

    [Header("縮放速度")]
    public float zoomSpeed = 10f;

    [Header("最小 FOV")]
    public float minFOV = 25f;

    [Header("最大 FOV")]
    public float maxFOV = 70f;

    [Header("上下視角限制")]
    public float minPitch = -60f;

    public float maxPitch = 60f;

    private float yaw;
    private float pitch;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        Vector3 angles = transform.eulerAngles;

        yaw = angles.y;
        pitch = angles.x;

        if (pitch > 180f)
        {
            pitch -= 360f;
        }
    }

    void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    void HandleRotation()
    {
        // 按住左鍵旋轉
        // 滑鼠在 UI 上時不旋轉
        if (Input.GetMouseButton(0)
            && !EventSystem.current.IsPointerOverGameObject())
        {
            float mouseX =
                Input.GetAxis("Mouse X") *
                rotationSpeed;

            float mouseY =
                Input.GetAxis("Mouse Y") *
                rotationSpeed;

            yaw += mouseX;

            pitch -= mouseY;

            pitch =
                Mathf.Clamp(
                    pitch,
                    minPitch,
                    maxPitch
                );

            transform.rotation =
                Quaternion.Euler(
                    pitch,
                    yaw,
                    0f
                );
        }
    }

    void HandleZoom()
    {
        if (cam == null)
            return;

        float scroll =
            Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.001f)
        {
            cam.fieldOfView -=
                scroll * zoomSpeed;

            cam.fieldOfView =
                Mathf.Clamp(
                    cam.fieldOfView,
                    minFOV,
                    maxFOV
                );
        }
    }

    // 導覽移動完後同步角度
    public void SyncRotation()
    {
        Vector3 angles = transform.eulerAngles;

        yaw = angles.y;
        pitch = angles.x;

        if (pitch > 180f)
        {
            pitch -= 360f;
        }
    }
}