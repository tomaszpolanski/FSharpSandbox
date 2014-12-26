using Luncher.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Launcher.Services.Universal
{
    public class FileSystemService : IFileSystemService
    {
        public async Task<string> ReadEmbeddedFileAsync(string fileName, CancellationToken token)
        {
            var file = await Package.Current.InstalledLocation.GetFileAsync(fileName).AsTask(token);
            return await FileIO.ReadTextAsync(file).AsTask(token);
        }
    }
}
