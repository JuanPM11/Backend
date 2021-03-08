using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Entidades;
using BackEnd.Utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/cines")]
    public class CinesController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public CinesController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<CineDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            //    logger.LogInformation("Mostrar los generos");
            //    return repositorio.ObtenerTodosLosGeneros();
            //PAGINACION EN API
            var queryable = context.Cines.AsQueryable();
            await HttpContext.InsertarPrametrosPaginacionEnCabecera(queryable);
            var cines = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<CineDTO>>(cines);


            //   return new List<Genero>() { new Genero() { Id = 1, Nombre = "Comedia" } };
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CineCreacionDTO cineCreacionDTO)
        {
            var cine = mapper.Map<Cine>(cineCreacionDTO);
            context.Add(cine);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{Id:int}")] //api/generos/2/Juan
        public async Task<ActionResult<CineDTO>> Get(int Id)
        {

            var cine = await context.Cines.FirstOrDefaultAsync(x => x.Id == Id);

            if (cine == null)
            {
                return NotFound();
            }
            return mapper.Map<CineDTO>(cine);

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

        [HttpPut("{Id:int}")]
        public async Task<ActionResult> Put(int Id, [FromBody] CineCreacionDTO cineCreacionDTO)
        {


            var cine = await context.Cines.FirstOrDefaultAsync(x => x.Id == Id);

            if (cine == null)
            {
                return NotFound();
            }

            cine = mapper.Map(cineCreacionDTO, cine);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var existe = await context.Cines.AnyAsync(x => x.Id == Id);

            if (!existe)
            {
                return NotFound();
            }
            context.Remove(new Cine() { Id = Id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
