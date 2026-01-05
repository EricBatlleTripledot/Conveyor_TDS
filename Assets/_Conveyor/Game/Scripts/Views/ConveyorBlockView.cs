using System;
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

		public ConveyorBlock ConveyorBlock => conveyorBlock;

		public void Initialize(ConveyorBlock conveyorBlock, SplineContainer splineContainer)
		{
			this.conveyorBlock = conveyorBlock;
			splineAnimate.Container = splineContainer;
			SetColor(conveyorBlock.Color);
		}

		public void Launch()
		{
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
	}
}