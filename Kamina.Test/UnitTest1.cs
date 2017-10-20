using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kamina.BL.Models;
using Kamina.BL.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kamina.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task Can_Create_New_Material()
        {
            FileSaverSettings settings = new FileSaverSettings()
            {
                Location =  "D:\\kamina"
            };

            var fileName = "File.txt";

            if (!File.Exists($"D:\\{fileName}"))
            {
                using (var writer = File.CreateText($"D:\\{fileName}"))
                {
                    writer.WriteLine(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                }
            }
            
            
            IFileSaver fileSaver = new LocalFileSaver(settings);

            MaterialsService service = new MaterialsService {FileSaver = fileSaver};

            MaterialBo newMaterial = new MaterialBo()
            {
                Category = new CategoryBo() { Id = Guid.Parse("b12070ae-cccb-4e25-ad51-3d56d19b5eb3") },
                Name = fileName
            };

            MaterialBo createdMaterial = null;
            

              using (var stream = File.OpenRead($"D:\\{fileName}"))
                {
                    createdMaterial = await service.CreateNewMaterial(newMaterial, stream);
                }
            
              Assert.IsNotNull(createdMaterial);

            MaterialBo existingMaterial = await service.GetMaterial(createdMaterial.Id);

            Assert.IsNotNull(existingMaterial);

            Assert.AreEqual(createdMaterial.Name, existingMaterial.Name);

            Assert.AreEqual(1, existingMaterial.Versions.Count());

            String firstVersionId = existingMaterial.Versions.FirstOrDefault()?.Id.ToString();

            bool newMaterialFileExist = File.Exists($"{settings.Location}\\{firstVersionId}");

            Assert.IsTrue(newMaterialFileExist);

            using (var md5Hash = MD5.Create())
            using (var fileStream = File.OpenRead($"D:\\{fileName}"))
            using (var existingFileStream = File.OpenRead($"{settings.Location}\\{firstVersionId}"))
            {
                var fileHashBytes = md5Hash.ComputeHash(fileStream);
                var existingFileHashBytes = md5Hash.ComputeHash(existingFileStream);

                var fileHash = Encoding.Default.GetString(fileHashBytes);
                var existingFileHash = Encoding.Default.GetString(existingFileHashBytes);
                Assert.AreEqual(fileHash, existingFileHash);
            }
        }


        [TestMethod]
        public async Task Can_Create_New_Version()
        {
            FileSaverSettings settings = new FileSaverSettings()
            {
                Location = "D:\\kamina"
            };

            var fileName = "File.txt";

            if (!File.Exists($"D:\\{fileName}"))
            {
                using (var writer = File.CreateText($"D:\\{fileName}"))
                {
                    writer.WriteLine(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                }
            }

            IFileSaver fileSaver = new LocalFileSaver(settings);

            MaterialsService service = new MaterialsService { FileSaver = fileSaver };
            
        }
    }
}
