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
        private float divisor;

        private void OnValidate()
        {
            UpdateCameraOrtographicSize();
        }

        private void UpdateCameraOrtographicSize()
        {
            var h = Screen.height;
            gameCamera.orthographicSize = h / divisor;
        }
    }
}