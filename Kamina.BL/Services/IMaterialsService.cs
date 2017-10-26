using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kamina.BL.Models;

namespace Kamina.BL.Services
{
    public interface IMaterialsService
    {
        IFileSaver FileSaver { get; set; }

        Task<MaterialBo> CreateNewMaterial(string fileName, Guid categoryId, Stream fileStream);
        Task<VersionBo> CreateNewMaterialVersion(Guid materialId, Stream fileStream);
        Task<IEnumerable<CategoryBo>> GetAllCategories();
        Task<SelectionContainer<MaterialBo>> GetAllMaterials(Guid categoryId, int skip = 0, int top = 50);
        Task<MaterialBo> GetMaterial(Guid materialId);
        Task<SelectionContainer<VersionBo>> GetMaterialVersions(Guid materialId, int skip = 0, int top = 50);
        Task<FileStreamInfo> GetVersionFile(Guid materialId, int versionNumber);
    }
}