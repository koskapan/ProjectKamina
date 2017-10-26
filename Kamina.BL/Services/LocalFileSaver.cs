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
        public FileSaverSettings Settings { get; }

        public LocalFileSaver(FileSaverSettings settings)
        {
            Settings = settings;
        }

        public async Task SaveFile(String fileId, Stream fileStream)
        {
            using (var file = File.Create($"{Settings.Location}\\{fileId}"))
            {
                fileStream.Seek(0, SeekOrigin.Begin);

                await fileStream.CopyToAsync(file);
            }
        }

        public async Task<Stream> GetFile(String fileId)
        {
            var filePath = $"{Settings.Location}\\{fileId}";

            if (File.Exists(filePath))
            {
                var fileStream = File.OpenRead(filePath);

                return fileStream;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
