using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Classes
{
    /// <summary>
    /// This class is responsible for managing the soundboard entries. This includes adding, removing, and editing entries by Group.
    /// </summary>
    public static class SoundboardEntryManager
    {
        struct SoundboardEntry
        {
            public string Name;
            public string Path;
            public string Group;
        }
    }
}
