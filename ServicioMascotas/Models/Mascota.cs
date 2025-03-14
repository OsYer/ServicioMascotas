using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServicioMascotas.Models
{
	public class Mascota
	{
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Especie { get; set; }
        public string Raza { get; set; }
        public int Edad { get; set; }
        public decimal Peso { get; set; }
        public char Sexo { get; set; }
        public int IdUsuario { get; set; }
    }
}