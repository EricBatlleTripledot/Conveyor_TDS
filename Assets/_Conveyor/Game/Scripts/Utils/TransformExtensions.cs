using System.Linq;
using UnityEngine;

namespace Game
{
	using UnityEngine;

	namespace Utils
	{
	}
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
		
		public static Transform DestroyAllChildsImmediate(this Transform transform)
		{
			var tempList = transform.Cast<Transform>().ToList();
			foreach(var child in tempList)
			{
				Object.DestroyImmediate(child.gameObject);
			}
			return transform;
		}
	}
}