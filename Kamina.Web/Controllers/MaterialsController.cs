using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Kamina.BL.Models;
using Kamina.BL.Services;

namespace Kamina.Web.Controllers
{
    [RoutePrefix("v1")]
    public class MaterialsController : ApiController
    {
        public IMaterialsService MaterialsService { get; set; }

        public MaterialsController(IMaterialsService materialsService)
        {
            MaterialsService = materialsService;
        }

        [HttpGet]
        [Route("materials/{categoryId:guid}/all")]
        [ResponseType(typeof(SelectionContainer<MaterialBo>))]
        public async Task<IHttpActionResult> GetMaterials(Guid categoryId, Int32 skip = 0, Int32 top = 50)
        {
            var resultMaterials = await MaterialsService.GetAllMaterials(categoryId, skip, top);

            return Ok(resultMaterials);
        }

        [HttpGet]
        [Route("materials/{materialId:guid}")]
        [ResponseType(typeof(MaterialBo))]
        public async Task<IHttpActionResult> GetMaterial(Guid materialId)
        {
            var resultMaterial = await MaterialsService.GetMaterial(materialId);

            if (resultMaterial == null)
            {
                return NotFound();
            }

            return Ok(resultMaterial);
        }

        [HttpPost]
        [Route("materials/{categoryId:guid}")]
        [ResponseType(typeof(MaterialBo))]
        public async Task<IHttpActionResult> CreateNewMaterial(Guid categoryId)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest();
            }

            var provider = new MultipartMemoryStreamProvider();

            await Request.Content.ReadAsMultipartAsync(provider);

            var file = provider.Contents.FirstOrDefault();

            if (file == null)
            {
                return Content(HttpStatusCode.NoContent, "no file");
            }

            var filename = file.Headers.ContentDisposition.FileName.Trim('\"');

            var fileStream = await file.ReadAsStreamAsync();

            var newMaterial = await MaterialsService.CreateNewMaterial(filename, categoryId, fileStream);

            return Ok(newMaterial);
        }



    }
}
