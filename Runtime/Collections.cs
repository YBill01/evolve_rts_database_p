/*generated: 2024-05-18 13:26*/

using C = EvolveRTS.Database.Collection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EvolveRTS.Database
{
	public class Collections
	{
		private Dictionary<Type, Dictionary<int, Element>> _dic;
		private Dictionary<int, Type> _dicTypes;

		public Collections()
		{
			_dic = new Dictionary<Type, Dictionary<int, Element>>();
			_dicTypes = new Dictionary<int, Type>();

			_dic.Add(typeof(C.SceneInfo), new Dictionary<int, Element>());


		}

		public T Get<T>(int id) where T : Element
		{
			return (T)_dic[typeof(T)][id];
		}
		public Type Get(int id)
		{
			return _dicTypes[id];
		}
		public Element Get(Type type, int id)
		{
			return _dic[type][id];
		}
		public Dictionary<int, T> Get<T>() where T : Element
		{
			return _dic[typeof(T)].ToDictionary(x => x.Key, x => x.Value as T);
		}

		internal void Read(BinaryReader binary)
		{
			int count;
			
			_dicTypes.Add(binary.ReadInt32(), typeof(C.SceneInfo));
			count = binary.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				C.SceneInfo element = C.SceneInfo.Read(binary, this);
				_dic[typeof(C.SceneInfo)].Add(element.id, element);
			}
			


		}

		internal void Clear()
		{
			_dic.Clear();
			_dicTypes.Clear();
		}
	}
}