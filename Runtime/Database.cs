using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace EvolveRTS.Database
{
	public static class Db
	{
		public static DateTime lastWrite { get; private set; }
		public static string gitBranchName { get; private set; }
		public static string appVersion { get; private set; }

		public static IDatabaseSerializer serializer { private get; set; }

		private static Collections _collections;
		public static Collections collections
		{
			get
			{
				return _collections ??= new Collections();
			}
		}

		public static bool isRead
		{
			get
			{
				return _collections is not null;
			}
		}

		public static void Read()
		{
			try
			{
				using (BinaryReader binary = new BinaryReader(serializer.GetStream()))
				{
					lastWrite = DateTime.ParseExact(binary.ReadString(), "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture);
					gitBranchName = binary.ReadString();
					appVersion = binary.ReadString();

					collections.Read(binary);

					Debug.Log($"<color=yellow>Database</color> reading <color=lime>completed.</color>");
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