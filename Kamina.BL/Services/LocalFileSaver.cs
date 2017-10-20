using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kamina.BL.Models;

namespace Kamina.BL.Services
{
    public class LocalFileSaver : IFileSaver
    {
        private FileSaverSettings _settings;

        public LocalFileSaver(FileSaverSettings settings)
        {
            _settings = settings;
        }

        public async Task SaveFile(String fileId, Stream fileStream)
        {
            using (var file = File.Create($"{_settings.Location}\\{fileId}"))
            {
                fileStream.Seek(0, SeekOrigin.Begin);

                await fileStream.CopyToAsync(file);
            }
        }

        public async Task<Stream> GetFile(String fileId)
        {
            using (var fileStream = File.OpenRead($"{_settings.Location}\\{fileId}"))
            {
                return fileStream;
            }
        }
    }
}
