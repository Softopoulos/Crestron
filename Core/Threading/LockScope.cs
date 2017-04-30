using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crestron.SimplSharp;
namespace Softopoulos.Crestron.Core.Threading
{
	public class LockScope : IDisposable
	{
		CCriticalSection _criticalSection;

		public LockScope(CCriticalSection criticalSection)
		{
			_criticalSection = criticalSection;
			_criticalSection.Enter();
		}

		private bool _disposed;

		public void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;
			_criticalSection.Leave();
		}
	}
}