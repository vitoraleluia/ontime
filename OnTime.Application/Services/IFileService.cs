using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OnTime.Application.Services;

public interface IFileService
{
    Task SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken = default);
    void EnsureDirectoryExists(string path);
    bool FileExists(string path);
    void DeleteFile(string path);
}
