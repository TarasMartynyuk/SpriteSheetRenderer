using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpriteSheetRendererExamples
{
public class CameraController : MonoBehaviour
{
    public static bool isBuildingState = false;

    public LayerMask IgnoreMe;
    public float zoomOutMax = 100;
    public float zoomOutMin = 10;


    public void MoveMainCamera(Vector3 touch)
    {
        Vector3 direction = touch - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Camera.main.transform.position += direction * Speed * Time.deltaTime;
    }


    private static void MoveScreenWhenCloseToBorder()
    {

        if ((Screen.width * 0.85 < Input.mousePosition.x) || (Input.mousePosition.x < Screen.width * 0.15))
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), 1f * Time.deltaTime);
        }
        if ((Screen.height * 0.85 < Input.mousePosition.y) || (Input.mousePosition.y < Screen.height * 0.01))
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), 1f * Time.deltaTime);
        }
    }

    [SerializeField] float MobileZoomSpeed;
    [SerializeField] float PCZoomSpeed;

    [SerializeField] float Speed;

    float m_zoomSpeed;
    Vector3 touchStart;

    private void Start()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            m_zoomSpeed = MobileZoomSpeed;
        }
        else
        {
            m_zoomSpeed = PCZoomSpeed;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.1f);
        }

        else if (Input.GetMouseButton(0) && !isBuildingState)
        {
            MoveMainCamera(touchStart);
        }
        if (isBuildingState)
        {
            MoveScreenWhenCloseToBorder();
        }

        zoom(Input.GetAxis("Mouse ScrollWheel"));
    }


    void zoom(float increment)
    {
        increment *= Time.deltaTime * m_zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }
}
}