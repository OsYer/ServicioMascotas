using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ServicioMascotas.Models
{
    [DataContract]
    public class Mascota
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public string Especie { get; set; }

        [DataMember]
        public string Raza { get; set; }

        [DataMember]
        public int Edad { get; set; }

        [DataMember]
        public decimal Peso { get; set; }

        [DataMember]
        public char Sexo { get; set; }

        [DataMember]
        public int IdUsuario { get; set; }

        [DataMember]
        public DateTime FechaRegistro { get; set; }

        [DataMember]
        public DateTime? FechaEdicion { get; set; }

        [DataMember]
        public bool Activo { get; set; }
    }
}