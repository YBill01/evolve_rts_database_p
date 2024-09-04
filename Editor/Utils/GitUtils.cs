using System.Diagnostics;
using UnityEditor.PackageManager;

namespace EvolveRTS.Editor.Database
{
	public static class GitUtils
	{
		private static string _currentBranchName = default;

		public static string GetBranchName(bool force = false)
		{
			if (string.IsNullOrEmpty(_currentBranchName) || force)
			{
				PackageInfo packageInfo = PackageInfo.FindForAssetPath(DatabaseConfig.ROOT_PATH);
				
				ProcessStartInfo startInfo = new("git");

				startInfo.UseShellExecute = false;
				startInfo.WorkingDirectory = packageInfo.resolvedPath;
				startInfo.RedirectStandardInput = true;
				startInfo.RedirectStandardOutput = true;
				startInfo.Arguments = "rev-parse --abbrev-ref HEAD";

				Process process = new();
				process.StartInfo = startInfo;
				process.Start();
				process.WaitForExit();

				_currentBranchName = process.StandardOutput.ReadLine();

				process.Close();
			}

			return _currentBranchName;
		}
	}
}