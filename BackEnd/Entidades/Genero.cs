using BackEnd.Validaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Entidades
{
    public class Genero 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 10)]
        [PrimeraLetraMayusculaAtributo]
        public string Nombre { get; set; }
        public List<PeliculasGeneros> PeliculasGeneros  { get; set; }

        /*  [Range(18, 120)]
          public int Edad { get; set; }

          [CreditCard]
          public string TarjetaDeCredito { get; set; }

          [Url]
          public string URL { get; set; }

          public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
          {
              if (!String.IsNullOrEmpty(Nombre))
              {
                  string primeraLetra = Nombre[0].ToString();
                  if (primeraLetra != primeraLetra.ToUpper())
                  {
                      yield return new ValidationResult("La primera letra debe ser mayyúscula", new string[] { nameof(Nombre)});
                  }
              }
          }*/
    }
}