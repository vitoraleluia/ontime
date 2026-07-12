using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OnTime.Application.Services;

namespace OnTime.Infrastructure.Services;

public class FileService : IFileService
{
    public async Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await stream.CopyToAsync(fileStream, cancellationToken);
    }

    public void EnsureDirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
