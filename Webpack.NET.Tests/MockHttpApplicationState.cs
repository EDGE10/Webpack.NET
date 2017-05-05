using System.Collections.Generic;
using System.Web;

namespace Webpack.NET.Tests
{
	class MockHttpApplicationState : HttpApplicationStateBase
	{
		private Dictionary<string, object> _values = new Dictionary<string, object>();

		public override void Lock() {}

		public override void UnLock() {}

		public override object this[string name] { get => _values[name]; set => _values[name] = value; }
	}
}
