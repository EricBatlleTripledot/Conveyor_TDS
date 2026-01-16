using UnityEngine;
using UnityEngine.Serialization;

namespace Game.BlockAnimation
{
    [CreateAssetMenu(
        fileName = "BlockAnimationSettings", 
        menuName = "Color Block Arrow/Block Animation Settings",
        order = 0)]
    // Settings are used across the ConveyorBlock and GridBlock, as they share animators + tweens
    public class BlockAnimationSettings : ScriptableObject
    {
        /*
         * Terms used:
         * Stack = the stack of tiles before a tile moves onto the Conveyor Belt
         * Belt = the conveyor belt
         * Board = the grid of waiting tiles
         * Rejected = when a conveyor belt tile tries to match but can't, and returns back onto the conveyor belt
         * Cascade = a chain of matches, where we start slow and build-up in speed per match
         */

        [Header("Init")]
        [SerializeField]
        private string initClipName = "ArrowTile_Init";
        
        [Header("From Stack onto Belt Animation")]
        [SerializeField]
        private BlockMotionConfig fromStackToBeltMotion;
        
        [Header("From Belt onto Board Animation")]
        [SerializeField]
        private BlockMotionConfig fromBeltToBoardMotion;
        
        [Header("From Belt but Rejected from Board Animation")]
        // is setup to be one animation with two stages of tween
        // 1 a movement to the board, akin to the motion above,
        // 2 then move backwards
        [SerializeField]
        private float fromBeltToRejectDuration = 1;
        [SerializeField]
        private float fromRejectToBeltDuration = 0.5f;
        [SerializeField]
        private float postRejectIdle = 0.1f;
        // note that the total duration of this animation is fromBeltToRejectDuration + fromRejectToBeltDuration
        [SerializeField]
        private AnimationCurve fromBeltToRejectMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        private AnimationCurve fromRejectToBeltMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        private string fromBeltToRejectClipName;

        [Header("Reject on Board")]
        [SerializeField]
        private float rejectOnBoardDuration = 0.5f;
        [SerializeField]
        private float rejectOnBoardStrength = 1f;
        [SerializeField]
        private int rejectOnBoardVibrato = 10;
        
        [Header("Chain-Blocking Animation")]
        [SerializeField]
        private string chainBlockerClipName = "Block_Reject";
        
        [Header("Cascade Animation")]
        // treated as an array of options to play through and repeat the last one until done,
        // if the cascade ends before this array is done, we always play the finish motion instead
        [SerializeField]
        private BlockMotionConfig[] cascadeMotions;
        [Space(10)]
        [SerializeField]
        private BlockMotionConfig finalCascadeMotion;
        // a different motion for when a chain is a low amount of tiles
        [SerializeField]
        private int thresholdForShorterCascadeFinishMotion = 1;
        [SerializeField]
        private BlockMotionConfig finalCascadeLowThresholdMotion;
        [Space(10)]
        [SerializeField]
        private float finalCascadeJumpDistance = 0.5f;
        
        [Header("Cascade Pre-empt Animation")]
        // for the chain of tiles that are about to be matched, do a small shudder
        // with a light delay down the chain
        [SerializeField]
        private float preEmptDelayPerIndex = 0.05f;
        [SerializeField]
        private string[] preEmptClipDirectionNames;

        public string InitClipName => initClipName;

        public BlockMotionConfig FromStackToBeltMotion => fromStackToBeltMotion;

        public BlockMotionConfig FromBeltToBoardMotion => fromBeltToBoardMotion;

        public float FromBeltToRejectDuration => fromBeltToRejectDuration;
        public float FromRejectToBeltDuration => fromRejectToBeltDuration;
        public float PostRejectIdle => postRejectIdle;
        public AnimationCurve FromBeltToRejectMoveCurve => fromBeltToRejectMoveCurve;
        public AnimationCurve FromRejectToBeltMoveCurve => fromRejectToBeltMoveCurve;
        public string FromBeltToRejectClipName => fromBeltToRejectClipName;

        public float RejectOnBoardDuration => rejectOnBoardDuration;
        public float RejectOnBoardStrength => rejectOnBoardStrength;
        public int RejectOnBoardVibrato => rejectOnBoardVibrato;

        public string ChainBlockerClipName => chainBlockerClipName;

        public BlockMotionConfig[] CascadeMotions => cascadeMotions;
        public BlockMotionConfig FinalCascadeMotion => finalCascadeMotion;
        public int ThresholdForShorterCascadeFinishMotion => thresholdForShorterCascadeFinishMotion;
        public BlockMotionConfig FinalCascadeLowThresholdMotion => finalCascadeLowThresholdMotion;
        public float FinalCascadeJumpDistance => finalCascadeJumpDistance;

        public bool ShouldDoShorterCascade(int cascadeIndex) => cascadeIndex <= ThresholdForShorterCascadeFinishMotion; 

        public float PreEmptDelayPerIndex => preEmptDelayPerIndex;
        public string PreEmptClipName(int directionIndex) => preEmptClipDirectionNames[directionIndex];
    }
}