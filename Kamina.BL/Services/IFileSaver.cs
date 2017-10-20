using System;
using System.IO;
using System.Threading.Tasks;

namespace Kamina.BL.Services
{
    public interface IFileSaver
    {
        Task SaveFile(String fileId, Stream fileStream);

        Task<Stream> GetFile(String fileId);
    }
}
