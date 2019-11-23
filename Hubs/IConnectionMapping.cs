using System.Collections.Generic;
using System.Linq;

namespace WebDekAPI.Hubs
{
	public interface IConnectionMapping<T>
	{
		int Count { get; }

		void Add(T key, string connectionId);


		IEnumerable<string> GetConnections(T key);


		void Remove(T key, string connectionId);
		
	}
}