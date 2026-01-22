using System;
using System.Threading.Tasks;
using Game.BlockAnimation;
using UnityEngine;
using UnityEngine.Splines;

namespace Game
{
	[RequireComponent(typeof(AlignedObjectRaycaster))]
	[RequireComponent(typeof(SplineAnimate))]
	public class ConveyorBlockView : MonoBehaviour
	{
		public event Action<ConveyorBlockView, GridBlockView> GridBlockDetected;
		
		[SerializeField]
		private ConveyorBlock conveyorBlock;
		[SerializeField]
		private AlignedObjectRaycaster alignedObjectRaycaster;
		[SerializeField]
		private SplineAnimate splineAnimate;
		[SerializeField]
		private MeshRenderer meshRenderer;
		[SerializeField]
		private BlockMotions tileMotions;

		public ConveyorBlock ConveyorBlock => conveyorBlock;
		public BlockMotions TileMotions => tileMotions;

		public void Initialize(ConveyorBlock conveyorBlock, SplineContainer splineContainer)
		{
			this.conveyorBlock = conveyorBlock;
			splineAnimate.Container = splineContainer;
			tileMotions.Initialise();
			SetColor(conveyorBlock.Color);
		}

		public async Task Launch(Vector3 point, float splineStartOffset)
		{
			await TileMotions.DoMoveOntoBelt(point).AsyncWaitForCompletion();
			// the animation of the launch lasts longer than the tween
			await TileMotions.WaitForAnimation();

			splineAnimate.StartOffset = splineStartOffset;
			splineAnimate.Play();

			alignedObjectRaycaster.EnableRaycasting = true;
		}

		private void Awake()
		{
			alignedObjectRaycaster.AlignedObjectDetected += OnAlignedObjectDetected;
		}

		public void Destroy()
		{
			splineAnimate.Pause();
			alignedObjectRaycaster.EnableRaycasting = false;
			Destroy(gameObject);
		}

		private void OnAlignedObjectDetected(RaycastHit hit)
		{
			var gridBlockView = hit.transform.GetComponent<GridBlockView>();
			if (!gridBlockView)
			{
				return;
			}
				
			GridBlockDetected?.Invoke(this, gridBlockView);
		}
		
		private void SetColor(Color color)
		{
			var propertyBlock = new MaterialPropertyBlock();
			propertyBlock.SetColor("_Color", color);
			meshRenderer.SetPropertyBlock(propertyBlock);
		}

		public void ToggleSplineMovement(bool value)
		{
			if (value)
			{
				splineAnimate.Play();
			}
			else
			{
				splineAnimate.Pause();
			}
		}

		public void ToggleDetection(bool value)
		{
			alignedObjectRaycaster.EnableRaycasting = value;
		}
	}
}