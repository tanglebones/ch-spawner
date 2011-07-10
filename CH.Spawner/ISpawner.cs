using System;
using System.Collections.Generic;

namespace CH.Spawner
{
    public interface ISpawner
    {
        IProgramInfo Spawn(IStartInfo startInfo);
        IEnumerable<IProgramInfo> Tracked { get; }
        bool Wait(TimeSpan timeout, params IProgramInfo[] programInfoArray);
    }
}