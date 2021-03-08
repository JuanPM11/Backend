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
    [Route("api/actores")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorAzureStorage almacenadorAzureStorage;
        private readonly string contenedor = "actores";


        public ActoresController(ApplicationDBContext context, IMapper mapper, IAlmacenadorAzureStorage almacenadorAzureStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorAzureStorage = almacenadorAzureStorage;
        }

        public IAlmacenadorAzureStorage AlmacenadorAzureStorage { get; }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await HttpContext.InsertarPrametrosPaginacionEnCabecera(queryable);
            var actores = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<ActorDTO>>(actores);

        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor == null)
            {
                return NotFound();
            }
            return mapper.Map<ActorDTO>(actor);


        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);
            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorAzureStorage.GuardarArchivo(contenedor, actorCreacionDTO.Foto);
            }
            context.Add(actor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            actor = mapper.Map(actorCreacionDTO, actor);

            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorAzureStorage.EditarArchivo(contenedor, actorCreacionDTO.Foto, actor.Foto);
            }

            await context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPost("buscarPorNombre")]
        public async Task<ActionResult<List<PeliculaActorDTO>>> BuscarPorNombre([FromBody]string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) { return new List<PeliculaActorDTO>(); }

            return await context.Actores
                .Where(x => x.Nombre.Contains(nombre))
                .Select(x=> new PeliculaActorDTO { Id = x.Id, Nombre = x.Nombre, Foto = x.Foto})
                .Take(5)
                .ToListAsync();

        }

        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == Id);



            if (actor == null)
            {
                return NotFound();
            }
            context.Remove(actor);
            await context.SaveChangesAsync();

            await almacenadorAzureStorage.BorrarArchivo(actor.Foto, contenedor);

            return NoContent();
        }
    }
}
