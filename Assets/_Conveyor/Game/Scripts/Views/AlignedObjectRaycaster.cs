using System;
using UnityEngine;

namespace Game
{
	[ExecuteInEditMode]
	public class AlignedObjectRaycaster : MonoBehaviour
	{
		public event Action<RaycastHit> AlignedObjectDetected;

		[SerializeField]
		private bool enableRaycasting = true;

		[SerializeField]
		private Vector3 rayCastDirection = Vector3.right;
		[SerializeField]
		private float alignMargin = 0.01f;
		[SerializeField]
		[Range(0, 1)]
		private float rotationTolerance = 0.01f;

		[Header("Raycast Colors")]
		[SerializeField]
		private Color noHitColor = Color.red;
		[SerializeField]
		private Color fullyAlignedColor = Color.green;
		[SerializeField]
		private Color hitNotAlignedColor = new Color(1f, 0.6470588f, 0.0f, 1f); // orange
		[SerializeField]
		private Color onlyPositionAlignedColor = new Color(0.627451f, 0.1254902f, 0.9411765f, 1f); // purple

		public bool EnableRaycasting
		{
			get => enableRaycasting;
			set => enableRaycasting = value;
		}

		private void Update()
		{
			if (!EnableRaycasting)
			{
				return;
			}

			if (Physics.Raycast(transform.position, transform.TransformDirection(rayCastDirection), out var hit, Mathf.Infinity))
			{
				if (IsAligned(transform.position, hit.transform.position, alignMargin))
				{
					if (IsParallelOrPerpendicular(transform, hit.transform, rotationTolerance))
					{
						Debug.DrawRay(transform.position, transform.TransformDirection(rayCastDirection) * hit.distance, fullyAlignedColor);
						AlignedObjectDetected?.Invoke(hit);
					}
					else
					{
						Debug.DrawRay(transform.position, transform.TransformDirection(rayCastDirection) * hit.distance, onlyPositionAlignedColor);
					}
				}
				else
				{
					Debug.DrawRay(transform.position, transform.TransformDirection(rayCastDirection) * hit.distance, hitNotAlignedColor);
				}
			}
			else
			{
				Debug.DrawRay(transform.position, transform.TransformDirection(rayCastDirection) * 1000, noHitColor);
			}
		}

		private bool IsParallelOrPerpendicular(Transform localTransform, Transform compareTransform, float rotationTolerance)
		{
			var dotProduct = Vector3.Dot(localTransform.forward, compareTransform.forward);
			var isParallel = Mathf.Abs(dotProduct) > 1f - rotationTolerance;
			var isPerpendicular = Mathf.Abs(dotProduct) < rotationTolerance;
			return isParallel || isPerpendicular;
		}
		
		private bool IsAligned(Vector3 localPos, Vector3 comparePos, float margin) => IsHorizontallyAligned(localPos, comparePos, margin) || IsVerticallyAligned(localPos, comparePos, margin);
		
		private bool IsHorizontallyAligned(Vector3 localPos, Vector3 comparePos, float margin) => Mathf.Abs(localPos.z - comparePos.z) <= margin;

		private bool IsVerticallyAligned(Vector3 localPos, Vector3 comparePos, float margin) => Mathf.Abs(localPos.x - comparePos.x) <= margin;
	}
}