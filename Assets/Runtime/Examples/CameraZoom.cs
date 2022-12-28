using UnityEngine;

namespace SpriteSheetRendererExamples
{
public class CameraZoom : MonoBehaviour {
  float maxFov = 90f;
  float minFov = 20;

  private void Start() {
    minFov = 0.000000001f;
  }

  void Update() {
    var fov = Camera.main.orthographicSize;
    fov += Input.GetAxis("Mouse ScrollWheel") * fov;
    fov = Mathf.Clamp(fov, minFov, maxFov);
    Camera.main.orthographicSize = fov;
  }
}
}