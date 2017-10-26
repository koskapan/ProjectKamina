using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Kamina.BL.Models;
using Kamina.BL.Services;

namespace Kamina.Web.Controllers
{
    [RoutePrefix("v1")]
    public class VersionsController : ApiController
    {
        public IMaterialsService MaterialsService { get; set; }

        public VersionsController(IMaterialsService materialsService)
        {
            MaterialsService = materialsService;
        }

        [Route("materials/{materialId:guid}/versions")]
        [HttpGet]
        [ResponseType(typeof(SelectionContainer<VersionBo>))]
        public async Task<IHttpActionResult> GetFileVersions(Guid materialId, Int32 skip =0, Int32 top = 50)
        {
            var versionsContainer = await MaterialsService.GetMaterialVersions(materialId, skip, top);

            return Ok(versionsContainer);
        }

        [Route("materials/{materialId:guid}/versions/{versionNumber}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetVersion(Guid materialId, Int32 versionNumber)
        {
            try
            {
                var fileStreamInfo = await MaterialsService.GetVersionFile(materialId, versionNumber);

                if (fileStreamInfo != null)
                {
                    return Stream(fileStreamInfo.FileStream, fileStreamInfo.FileName);
                }
            }
            catch (FileNotFoundException ex)
            {
                return NotFound();
            }

            return NotFound();
        }

        [Route("materials/{materialId:guid}/versions")]
        [HttpPost]
        [ResponseType(typeof(SelectionContainer<VersionBo>))]
        public async Task<IHttpActionResult> CreateVersion(Guid materialId)
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
            
            var fileStream = await file.ReadAsStreamAsync();

            var newMaterialVersion = await MaterialsService.CreateNewMaterialVersion(materialId, fileStream);

            return Ok(newMaterialVersion);
        }

        private IHttpActionResult Stream(Stream fileStream, String fileName)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(fileStream)
            };

            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return ResponseMessage(result);
        }

    }
}
