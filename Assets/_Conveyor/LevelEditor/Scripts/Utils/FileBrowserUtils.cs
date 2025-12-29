#nullable enable
using System.Threading.Tasks;
using SimpleFileBrowser;

namespace LevelEditor
{
	public static class FileBrowserUtils
	{
		public static Task<string?> PickFolderToSaveAsync()
		{
			var tcs = new TaskCompletionSource<string?>();

			var completed = false;

			FileBrowser.ShowSaveDialog(
				onSuccess: (results) =>
				{
					if (completed) return;
					completed = true;

					var path = results is { Length: > 0 } ? results[0] : null;
					tcs.TrySetResult(path);
				},
				onCancel: () =>
				{
					if (completed) return;
					completed = true;

					tcs.TrySetResult(null);
				},
				pickMode: FileBrowser.PickMode.Folders
			);

			return tcs.Task;
		}
	
		public static Task<string?> PickFileToLoadAsync()
		{
			var tcs = new TaskCompletionSource<string?>();

			var completed = false;

			FileBrowser.ShowLoadDialog(
				onSuccess: (results) =>
				{
					if (completed) return;
					completed = true;

					var path = results is { Length: > 0 } ? results[0] : null;
					tcs.TrySetResult(path);
				},
				onCancel: () =>
				{
					if (completed) return;
					completed = true;

					tcs.TrySetResult(null);
				},
				pickMode: FileBrowser.PickMode.Files
			);

			return tcs.Task;
		}
	}
}