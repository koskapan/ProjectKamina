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
                Location = "D:\\kamina"
            };

            var fileName = $"{settings.Location}\\File.txt";

            if (!File.Exists(fileName))
            {
                using (var writer = File.CreateText(fileName))
                {
                    writer.WriteLine(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                }
            }


            IFileSaver fileSaver = new LocalFileSaver(settings);

            IMaterialsService service = new MaterialsService(fileSaver);
            
            MaterialBo createdMaterial = null;

            using (var stream = File.OpenRead(fileName))
            {
                createdMaterial = await service.CreateNewMaterial(fileName, Guid.Parse("b12070ae-cccb-4e25-ad51-3d56d19b5eb3"), stream);
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
            using (var fileStream = File.OpenRead(fileName))
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
        public async Task Can_Get_Materials()
        {
            FileSaverSettings settings = new FileSaverSettings()
            {
                Location = "D:\\kamina"
            };
            
            IFileSaver fileSaver = new LocalFileSaver(settings);

            IMaterialsService service = new MaterialsService(fileSaver);

            var categoryId = Guid.Parse("b12070ae-cccb-4e25-ad51-3d56d19b5eb3");

            var materialsContainer = await service.GetAllMaterials(categoryId);

            Assert.IsNotNull(materialsContainer);

            if (materialsContainer.Count > 0)
            {
                foreach (var materialsContainerItem in materialsContainer.Items)
                {
                    Assert.AreNotEqual(0, materialsContainerItem.Versions.Count());

                    Assert.AreEqual(materialsContainerItem.Versions.Count(), materialsContainerItem.Versions.Max(v => v.VersionNumber));
                }
            }

        }

        [TestMethod]
        public async Task Can_Create_New_Version()
        {
            FileSaverSettings settings = new FileSaverSettings()
            {
                Location = "D:\\kamina"
            };

            var fileName = $"{settings.Location}\\File.txt";

            if (!File.Exists(fileName))
            {
                using (var writer = File.CreateText(fileName))
                {
                    writer.WriteLine(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                }
            }

            IFileSaver fileSaver = new LocalFileSaver(settings);

            IMaterialsService service = new MaterialsService(fileSaver);
            
            MaterialBo createdMaterial = null;

            using (var stream = File.OpenRead(fileName))
            {
                createdMaterial = await service.CreateNewMaterial(fileName, Guid.Parse("b12070ae-cccb-4e25-ad51-3d56d19b5eb3"), stream);
            }



            for (int i = 0; i < 5; i++)
            {

                var versionFileName = $"{settings.Location}\\file{i}.txt";

                using (var writer = File.CreateText(versionFileName))
                {
                    writer.WriteLine(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                }

                using (var versionFIleStream = File.OpenRead(versionFileName))
                {
                    var createdVersion = await service.CreateNewMaterialVersion(createdMaterial.Id,
                        versionFIleStream);

                    Assert.IsNotNull(createdVersion);

                    var createdVersionMaterial = await service.GetMaterial(createdMaterial.Id);

                    Assert.AreEqual(i + 2, createdVersionMaterial.Versions.Count());

                    Assert.AreEqual(i + 2, createdVersion.VersionNumber);

                    var fileStreamInfo = await service.GetVersionFile(createdMaterial.Id, createdVersion.VersionNumber);

                    using (var md5Hash = MD5.Create())
                    using (var fileStream = File.OpenRead(versionFileName))
                    using (var existingFileStream = fileStreamInfo.FileStream)
                    {
                        var fileHashBytes = md5Hash.ComputeHash(fileStream);
                        var existingFileHashBytes = md5Hash.ComputeHash(existingFileStream);

                        var fileHash = Encoding.Default.GetString(fileHashBytes);
                        var existingFileHash = Encoding.Default.GetString(existingFileHashBytes);
                        Assert.AreEqual(fileHash, existingFileHash);
                    }
                }


            }



        }
        
    }
}
