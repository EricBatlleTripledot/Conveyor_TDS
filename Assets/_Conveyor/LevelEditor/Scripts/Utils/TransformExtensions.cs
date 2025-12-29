using UnityEngine;

namespace LevelEditor
{
	public static class TransformExtensions
	{
		public static Transform DestroyAllChilds(this GameObject go)
		{
			foreach (Transform child in go.transform)
			{
				Object.Destroy(child.gameObject);
			}
			return go.transform;
		}

		public static Transform DestroyAllChilds(this Transform transform)
		{
			foreach (Transform child in transform)
			{
				Object.Destroy(child.gameObject);
			}
			return transform;
		}
	}
}