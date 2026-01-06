using UnityEngine;
using UnityEngine.Splines;

namespace Game
{
	public class ConveyorBlockViewFactory
	{
		private readonly ConveyorBlockView conveyorBlockViewPrefab;
		private readonly SplineContainer splineContainer;

		public ConveyorBlockViewFactory(ConveyorBlockView conveyorBlockViewPrefab, SplineContainer splineContainer)
		{
			this.conveyorBlockViewPrefab = conveyorBlockViewPrefab;
			this.splineContainer = splineContainer;
		}

		public ConveyorBlockView Create(ConveyorBlock conveyorBlock)
		{
			var conveyorBlockView = Object.Instantiate(conveyorBlockViewPrefab, splineContainer.transform);
			conveyorBlockView.Initialize(conveyorBlock, splineContainer);
			return conveyorBlockView;
		}
	}
}