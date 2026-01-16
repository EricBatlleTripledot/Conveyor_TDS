using UnityEngine;

namespace Game
{
    public class FrameRateService
    {
        public const int DEFAULT_FRAME_RATE = 120;

        public void SetTargetFrameRate(int fps = DEFAULT_FRAME_RATE)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = fps;
        }
    }
}