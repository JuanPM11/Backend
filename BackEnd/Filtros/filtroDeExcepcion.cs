using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Filtros
{
    public class filtroDeExcepcion : ExceptionFilterAttribute
    {
        private readonly ILogger<filtroDeExcepcion> logger;

        public filtroDeExcepcion(ILogger<filtroDeExcepcion> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)

        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }

    }
}
