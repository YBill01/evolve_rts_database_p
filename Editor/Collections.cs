/*generated: 2024-05-18 13:26*/

using C = EvolveRTS.Editor.Database.Collection;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace EvolveRTS.Editor.Database
{
	public class Collections
	{
		private Dictionary<Type, ISOCollection> _dic;
		
		public Collections()
		{
			_dic = new Dictionary<Type, ISOCollection>();

			_dic.Add(typeof(C.SceneInfo), AssetDatabase.LoadAssetAtPath<C.Scenes>(@"Packages/com.ybgames.evolve_rts.database/Editor/SOCollections/SOAssets\__Core\Scenes.asset"));


		}

		public Collection<T> Get<T>() where T : Element, new()
		{
			return (Collection<T>)_dic[typeof(T)];
		}
		public ISOCollection Get(Type type)
		{
			if (_dic.TryGetValue(type, out ISOCollection collection))
			{
				return collection;
			}

			return default;
		}
		public T Get<T>(int id) where T : Element, new()
		{
			List<T> elements = ((Collection<T>)_dic[typeof(T)]).elements;
			foreach (T element in elements)
			{
				if (element.id == id)
				{
					return element;
				}
			}

			return default;
		}
		
		internal void Write(BinaryWriter binary)
		{
			((C.Scenes)_dic[typeof(C.SceneInfo)]).Write(binary);


		}

		internal void Clear()
		{
			_dic.Clear();
		}
	}
}