using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ResXModifier
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                DisplayHelp();
                return;
            }

            var resXFileToModify = args[0];
            var configurationFile = args[1];
            var modifiedResXFile = args[2];

            if ((!File.Exists(resXFileToModify)) ||
                (!File.Exists(configurationFile)))
            {
                DisplayHelp();
                return;
            }

            var modifier = new Modifier();
            modifier.loadResXFileToModify(resXFileToModify);
            if (modifier.tryModify(configurationFile))
            {
                modifier.writeModifiedResX(modifiedResXFile);
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("ResXModifier <resXFileToModify.resx> <ConfigurationFile.xml> <modifiedResXFile>");
        }
    }
}
