using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
	public class JsonUtilityUtils
	{
		[Serializable]
		private class Wrapper<T>
		{
			public T[] Items;
		}

		public static string ToJson<T>(List<T> list, bool prettyPrint = false)
		{
			var wrapper = new Wrapper<T> { Items = list?.ToArray() ?? Array.Empty<T>() };
			return JsonUtility.ToJson(wrapper, prettyPrint);
		}

		public static List<T> FromJson<T>(string json)
		{
			if (string.IsNullOrEmpty(json))
				return new List<T>();

			var wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
			return wrapper?.Items != null ? new List<T>(wrapper.Items) : new List<T>();
		}
	}
}