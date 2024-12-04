using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Systems
{
    public abstract class LoadedClankFile 
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public LoadedClankFile(string name, string path) 
        {
            Name = name;
            Path = path;
        }
    }

    public class SoundboardFile : LoadedClankFile
    {
        public SoundboardFile(string name, string path) : base(name, path) 
        {

        }
    }


}
