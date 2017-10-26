using System;
using System.CodeDom;
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
    public class MaterialsService : IMaterialsService
    {
        public IFileSaver FileSaver { get; set; }

        public MaterialsService(IFileSaver fileSaver)
        {
            FileSaver = fileSaver;
        }

        public async Task<MaterialBo> CreateNewMaterial(String fileName, Guid categoryId, Stream fileStream)
        {
            using (var db = new KaminaDbContext())
            {

                var dbMaterial = new Material()
                {
                    CategoryId = categoryId,
                    Name = fileName,
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
                    Id = createdDbMaterial.Id,
                    Name = createdDbMaterial.Name
                };

                createdMaterial.Versions = new List<VersionBo>() { new VersionBo()
                {
                    Id = createdDbVersion.Id,
                    VersionSize = createdDbVersion.VersionSize,
                    VersionNumber = createdDbVersion.VersionNumber,
                    CreateDate = createdDbVersion.CreateDate
                }};

                if (materialDbCategory != null)
                {
                    createdMaterial.Category = new CategoryBo()
                    {
                        Id = materialDbCategory.Id,
                        Name = materialDbCategory.Name
                    };
                }

                return createdMaterial;
            }
        }

        public async Task<SelectionContainer<MaterialBo>> GetAllMaterials(Guid categoryId, Int32 skip = 0, Int32 top = 50)
        {
            var resultMaterials = new List<MaterialBo>();

            var totalCount = 0;

            using (var db = new KaminaDbContext())
            {
                var baseQuery = db.Materials.AsNoTracking().Where(m => m.CategoryId == categoryId);

                totalCount = await baseQuery.CountAsync();

                var dbMaterials = await baseQuery.OrderBy(m => m.Id).Skip(skip).Take(top).ToListAsync();

                var materialIds = dbMaterials.Select(m => m.Id);

                var materialsVersions = await db.Versions.AsNoTracking().Where(v => materialIds.Contains(v.MaterialId)).ToListAsync();

                var materialCategoryIds = dbMaterials.Select(m => m.CategoryId);

                var dbCategories = await db.Categories.AsNoTracking().Where(c => materialCategoryIds.Contains(c.Id)).ToListAsync();

                resultMaterials = dbMaterials.Select(m => new MaterialBo()
                {
                    Id = m.Id,
                    Name = m.Name,
                    Versions = materialsVersions.Where(v => v.MaterialId == m.Id).Select(v => new VersionBo()
                    {
                        Id = v.Id,
                        VersionSize = v.VersionSize,
                        VersionNumber = v.VersionNumber,
                        CreateDate = v.CreateDate
                    }).OrderBy(v => v.VersionNumber).ToList(),
                    Category = dbCategories.Where(c => c.Id == m.CategoryId).Select(c => new CategoryBo()
                    {
                      Id  = c.Id,
                      Name = c.Name
                    }).FirstOrDefault()
                }).ToList();
            }

            var resultContainer = new SelectionContainer<MaterialBo>()
            {
                Count = totalCount,
                Skip = skip,
                Items = resultMaterials
            };

            return resultContainer;
        }

        public async Task<MaterialBo> GetMaterial(Guid materialId)
        {
            using (var db = new KaminaDbContext())
            {
                var dbMaterial = await db.Materials.AsNoTracking().FirstOrDefaultAsync(m => m.Id == materialId);

                if (dbMaterial == null)
                {
                    return null;
                }

                var dbCategory = await db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == dbMaterial.CategoryId);

                var dbVersions  = await db.Versions.AsNoTracking().Where(v => v.MaterialId == dbMaterial.Id).ToListAsync();

                var foundMaterial = new MaterialBo()
                {
                    Id = dbMaterial.Id,
                    Name = dbMaterial.Name,
                    Versions = dbVersions.Select(v => new VersionBo()
                    {
                        Id = v.Id,
                        VersionSize = v.VersionSize,
                        VersionNumber = v.VersionNumber,
                        CreateDate = v.CreateDate
                    }).OrderBy(v => v.VersionNumber).ToList()
                };

                if (dbCategory != null)
                {
                    foundMaterial.
                        Category = new CategoryBo()
                    {
                        Id = dbCategory.Id,
                        Name = dbCategory.Name
                    };
                }

                return foundMaterial;

            }
        }

        public async Task<VersionBo> CreateNewMaterialVersion(Guid materialId, Stream fileStream)
        {
            VersionBo addedVersion = null;

            using (var db = new KaminaDbContext())
            {
                var materialVersions = await db.Versions.Where(v => v.MaterialId == materialId).ToListAsync();


                if (materialVersions.Any())
                {

                    var lastVersionNumber = materialVersions.Max(v => v.VersionNumber);

                    var dbVersion = new MaterialVersion()
                    {
                        Id = Guid.NewGuid(),
                        VersionSize = fileStream.Length,
                        CreateDate = DateTime.UtcNow,
                        MaterialId = materialId,
                        VersionNumber = lastVersionNumber + 1
                    };

                    var addedDbVersion = db.Versions.Add(dbVersion);

                    await FileSaver.SaveFile(addedDbVersion.Id.ToString(), fileStream);

                    addedVersion = new VersionBo()
                    {
                        Id = addedDbVersion.Id,
                        VersionNumber = addedDbVersion.VersionNumber,
                        VersionSize = addedDbVersion.VersionSize,
                        CreateDate = addedDbVersion.CreateDate
                    };

                    await  db.SaveChangesAsync();
                }
            }

            return addedVersion;
        }

        public async Task<SelectionContainer<VersionBo>> GetMaterialVersions(Guid materialId, Int32 skip = 0, Int32 top = 50)
        {
            var totalCount = 0;

            using (var db = new KaminaDbContext())
            {
                var baseQuery = db.Versions.AsNoTracking().Where(v => v.MaterialId == materialId);

                totalCount = await baseQuery.CountAsync();

                var resultDbVersions = await baseQuery.Skip(skip).Take(top).OrderBy(v => v.VersionNumber).ToListAsync();

                var resultVersions = resultDbVersions.Select(v => new VersionBo()
                {
                    Id = v.Id,
                    VersionNumber = v.VersionNumber,
                    VersionSize = v.VersionSize,
                    CreateDate = v.CreateDate
                });

                var result = new SelectionContainer<VersionBo>()
                {
                    Count = totalCount,
                    Skip = skip,
                    Items = resultVersions.ToList()
                };

                return result;
            }
        }

        public async Task<IEnumerable<CategoryBo>> GetAllCategories()
        {
            using (var db = new KaminaDbContext())
            {
                var dbCategories = await db.Categories.AsNoTracking().ToListAsync();

                var resultCategories = dbCategories.Select(c => new CategoryBo() {Id = c.Id, Name = c.Name});

                return resultCategories;
            }
        }

        public async Task<FileStreamInfo> GetVersionFile(Guid materialId, Int32 versionNumber)
        {
            using (var db = new KaminaDbContext())
            {
                var material = await db.Materials.FirstOrDefaultAsync(m => m.Id == materialId);

                if (material == null)
                {
                    return null;
                }
                var dbVersion = await db.Versions.AsNoTracking().FirstOrDefaultAsync(v => v.MaterialId == materialId && v.VersionNumber == versionNumber);

                if (dbVersion == null)
                {
                    return null;
                }

                var fileName = material.Name;
                
                var fileStream = await FileSaver.GetFile(dbVersion.Id.ToString());

                return new FileStreamInfo()
                {
                    FileStream = fileStream,
                    FileName = fileName
                };

            }
        }
        
    }
}
