using System;
using System.IO;
using System.Threading.Tasks;
using Kamina.BL.Models;

namespace Kamina.BL.Services
{
    public interface IFileSaver
    {
        FileSaverSettings Settings { get; }

        Task SaveFile(String fileId, Stream fileStream);

        Task<Stream> GetFile(String fileId);
    }
}
