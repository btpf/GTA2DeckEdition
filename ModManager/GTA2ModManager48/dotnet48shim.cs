using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA2ModManager48
{
    public class dotnet48shim
    {
        public static string GetRelativePath(string basePath, string targetPath)
        {
            if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException(nameof(basePath));
            if (string.IsNullOrEmpty(targetPath)) throw new ArgumentNullException(nameof(targetPath));

            Uri baseUri = new Uri(AppendTrailingSlash(Path.GetFullPath(basePath)));
            Uri targetUri = new Uri(Path.GetFullPath(targetPath));

            if (baseUri.Scheme != targetUri.Scheme)
            {
                // Different drive letters or schemes — no relative path possible
                return targetPath;
            }

            Uri relativeUri = baseUri.MakeRelativeUri(targetUri);
            return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private static string AppendTrailingSlash(string path)
        {
            // Ensures directories end with a slash so URIs behave correctly
            if (!Path.HasExtension(path) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }
            return path;
        }
    }
}
