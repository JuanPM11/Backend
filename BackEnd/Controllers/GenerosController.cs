using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Entidades;
using BackEnd.Filtros;
using BackEnd.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [Route("api/generos")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class GenerosController : ControllerBase
    {
        // private readonly IRepositorio repositorio;
        private readonly ILogger<GenerosController> logger;
        private readonly ApplicationDBContext context;
        private readonly Mapper mapper;


        public GenerosController(ILogger<GenerosController> logger, ApplicationDBContext context, IMapper mapper)
        {

            //this.repositorio = repositorio;
            this.logger = logger;
            this.context = context;
            this.mapper = (Mapper)mapper;
        }
        [HttpGet] // api/generos
                  //[HttpGet("listado")]//api/generos/listado
                  // [HttpGet("/listadogeneros")]// /listadogeneros
                  // [ResponseCache(Duration = 60)] // filtro caché
                  // [ServiceFilter(typeof(MiFiltroDeAccion))]
        public async Task<ActionResult<List<GeneroDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            //    logger.LogInformation("Mostrar los generos");
            //    return repositorio.ObtenerTodosLosGeneros();
            //PAGINACION EN API
            var queryable = context.Generos.AsQueryable();
            await HttpContext.InsertarPrametrosPaginacionEnCabecera(queryable);
            var generos = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<GeneroDTO>>(generos);


            //   return new List<Genero>() { new Genero() { Id = 1, Nombre = "Comedia" } };
        }


        /*  [HttpGet("guid")] // api/generos/guid
          public ActionResult<Guid> Getguid()
          {
              return Ok(new { GUID_GenerosController = repositorio.obtenerGuid() });
          }*/

        [HttpGet("{Id:int}")] //api/generos/2/Juan
        public async Task<ActionResult<GeneroDTO>> Get(int Id)
        {

            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == Id);

            if (genero == null)
            {
                return NotFound();
            }
            return mapper.Map<GeneroDTO>(genero);

            //   logger.LogDebug($"Obteniendo un genero por el id {Id}");
            /*  var genero = await repositorio.ObtenerId(Id);

              if (genero == null)
              {
                  throw new ApplicationException("$ El género de ID {Id} no fué encontrado");
                  logger.LogWarning($"No se pudo encontrar el genero de id {Id}");
                  return NotFound();
              }

              return genero;*/


        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {

            //repositorio.crearGenero(genero);
            // return NoContent();
            var genero = mapper.Map<Genero>(generoCreacionDTO);
            context.Add(genero);
            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpPut("{Id:int}")]
        public async Task<ActionResult> Put(int Id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {

            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == Id);

            if (genero == null)
            {
                return NotFound();
            }

            genero = mapper.Map(generoCreacionDTO, genero);

            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == Id);

            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Genero() { Id = Id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
