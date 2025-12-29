using System.IO;
using UnityEngine;

namespace LevelEditor
{
	public static class JsonFileUtils
	{
		public static void SaveJsonToFile(string fileName, string json)
		{
			var path = Path.Combine(Application.persistentDataPath, fileName);
			File.WriteAllText(path, json);
			Debug.Log($"[{nameof(JsonFileUtils)}] Saved JSON to: {path}");
		}

		public static string LoadJsonFromFile(string fileName)
		{
			var path = Path.Combine(Application.persistentDataPath, fileName);

			if (File.Exists(path))
			{
				return File.ReadAllText(path);
			}
			Debug.LogWarning($"[{nameof(JsonFileUtils)}] File not found: {path}");
			return null;
		}
	}
}