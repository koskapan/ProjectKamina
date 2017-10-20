using System;
using System.Collections;
using System.Threading.Tasks;
using Kamina.BL.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Kamina.DAL.Models;

namespace Kamina.BL.Services
{
    public class MaterialsService
    {
        public IFileSaver FileSaver { get; set; }

        public async Task<MaterialBo> CreateNewMaterial(MaterialBo material, Stream fileStream)
        {
            using (var db = new KaminaDbContext())
            {
                var dbMaterial = new Material()
                {
                    CategoryId = material.Category.Id,
                    Name = material.Name,
                    Id = Guid.NewGuid()
                };

                var firstVersion = new MaterialVersion()
                {
                    Id = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    MaterialId = dbMaterial.Id,
                    VersionNumber = 1,
                    VersionSize = fileStream.Length
                };

                var createdDbMaterial = db.Materials.Add(dbMaterial);
                var createdDbVersion = db.Versions.Add(firstVersion);
                var materialDbCategory = db.Categories.FirstOrDefault(c => c.Id == createdDbMaterial.CategoryId);

                await db.SaveChangesAsync();

                await FileSaver.SaveFile(firstVersion.Id.ToString(), fileStream);



                var createdMaterial = new MaterialBo()
                {
                    Id = createdDbMaterial.CategoryId,
                    Name = createdDbMaterial.Name
                };

                createdMaterial.Versions = new List<VersionBo>() { new VersionBo()
                {
                    Id = createdDbVersion.Id,
                    VersionSize = createdDbVersion.VersionSize,
                    VersionNumber = createdDbVersion.VersionNumber,
                    CreateDate = createdDbVersion.CreateDate
                }};

                createdMaterial.Category = new CategoryBo()
                {
                    Id = materialDbCategory.Id,
                    Name = materialDbCategory.Name
                };

                return createdMaterial;
            }
        }

        public async Task<IEnumerable<MaterialBo>> GetAllMaterials(Guid categoryId, Int32 skip = 0, Int32 top = 50)
        {
            var resultMaterials = new List<MaterialBo>();

            using (var db = new KaminaDbContext())
            {
                var dbMaterials = await db.Materials.Where(m => m.CategoryId == categoryId).OrderBy(m => m.Id).Skip(skip).Take(top).ToListAsync();

                resultMaterials = dbMaterials.Select(m => GetMaterial(m.Id).Result).ToList();
            }

            return resultMaterials;
        }

        public async Task<MaterialBo> GetMaterial(Guid materialId)
        {
            using (var db = new KaminaDbContext())
            {
                var dbMaterial = db.Materials.FirstOrDefault();

                if (dbMaterial == null)
                {
                    return null;
                }

                var dbCategory = db.Categories.FirstOrDefault(c => c.Id == dbMaterial.CategoryId);

                var dbVersions  = await db.Versions.Where(v => v.MaterialId == dbMaterial.Id).ToListAsync();

                var foundMaterial = new MaterialBo()
                {
                    Id = dbMaterial.Id,
                    Name = dbMaterial.Name,
                    Category = new CategoryBo()
                    {
                        Id = dbCategory.Id,
                        Name = dbCategory.Name
                    },
                    Versions = dbVersions.Select(v => new VersionBo()
                    {
                        Id = v.Id,
                        VersionSize = v.VersionSize,
                        VersionNumber = v.VersionNumber,
                        CreateDate = v.CreateDate
                    }).OrderBy(v => v.VersionNumber)
                };

                return foundMaterial;

            }
        }

        public async Task<MaterialBo> CreateNewMaterialVersion(VersionBo materialVersion, Guid materialId, Stream fileStream)
        {
            using (var db = new KaminaDbContext())
            {
                var lastVersionNumber = db.Versions.Where(v => v.MaterialId == materialId).Max(v => v.VersionNumber);

                var dbVersion = new MaterialVersion()
                {
                    Id = Guid.NewGuid(),
                    VersionSize = fileStream.Length,
                    CreateDate = DateTime.UtcNow,
                    MaterialId = materialId,
                    VersionNumber = lastVersionNumber + 1
                };

                var addedVersion = db.Versions.Add(dbVersion);

                await FileSaver.SaveFile(addedVersion.Id.ToString(), fileStream);

                await db.SaveChangesAsync();
            }

            return await GetMaterial(materialId);
        }
        
        public async Task<Stream> GetVersionFile(Guid versionId)
        {
            return await FileSaver.GetFile(versionId.ToString());
        }
    }
}
