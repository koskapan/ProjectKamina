using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Kamina.BL.Models;
using Kamina.BL.Services;


namespace Kamina.Web.Controllers
{
    [RoutePrefix("v1/categories")]
    public class CategoriesController : ApiController
    {
     
        public IMaterialsService MaterialsService { get; set; }

        public CategoriesController(IMaterialsService materialsService)
        {
            MaterialsService = materialsService;
        }
        
        [HttpGet]
        [Route()]
        [ResponseType(typeof(SelectionContainer<CategoryBo>))]
        public async Task<IHttpActionResult> GetCategories()
        {
            var categories = await MaterialsService.GetAllCategories();

            var categoryBoArray = categories as CategoryBo[] ?? categories.ToArray();

            var result = new SelectionContainer<CategoryBo>()
            {
                Skip = 0,
                Count = categoryBoArray.Count(),
                Items = categoryBoArray.ToList()
            };

            return Ok(result);
        }
    }
}
