using System;
using System.IO;
using UnityEngine;

namespace EvolveRTS.Editor.Database
{
	public static class Db
	{
		private static Collections _collections;
		public static Collections collections
		{
			get
			{
				return _collections ??= new Collections();
			}
		}

		public static void Write()
		{
			try
			{
				if (!Directory.Exists(Path.Combine("../", DatabaseSettings.Instance.savePath)))
				{
					Directory.CreateDirectory(Path.Combine("../", DatabaseSettings.Instance.savePath));
				}

				FileStream fileStream = new FileStream(Path.Combine("../", DatabaseSettings.Instance.savePath, DatabaseSettings.Instance.saveName), FileMode.Create);
				using (BinaryWriter binary = new BinaryWriter(fileStream))
				{
					binary.Write(DateTime.UtcNow.ToString("yyyy.MM.dd HH:mm"));
					binary.Write(GitUtils.GetBranchName());
					binary.Write(Application.version);

					collections.Write(binary);

					Debug.Log($"<color=yellow>Database</color> write. File: <color=lime>{fileStream.Name}</color>");
				}
			}
			catch (Exception error)
			{
				Debug.LogException(error);
			}
		}

		public static void Clear()
		{
			_collections.Clear();
			_collections = null;
		}
	}
}