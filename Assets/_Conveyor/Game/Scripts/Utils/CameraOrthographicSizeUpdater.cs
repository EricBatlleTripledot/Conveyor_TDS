using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace Game
{
    [ExecuteInEditMode]
    public class CameraOrthographicSizeUpdater : MonoBehaviour
    {
        [SerializeField]
        private Camera gameCamera;
        [SerializeField]
        private float divisor = 160;

        private void OnValidate()
        {
            UpdateCameraOrtographicSize();
        }

        private void UpdateCameraOrtographicSize()
        {
            if (gameCamera == null) {
                return;
            }

            var h = Screen.currentResolution.height;
            gameCamera.orthographicSize = h / divisor;
        }
    }
}