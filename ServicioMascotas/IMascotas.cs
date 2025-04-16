using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ServicioMascotas.Models;

namespace ServicioMascotas
{
    [ServiceContract]
    public interface IMascotas
    {
        // Método para probar la conexión
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "probarconexion")]
        string ProbarConexion();

        // Método para agregar una nueva mascota
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "agregarmascota")]
        ResultadoOperacion AgregarMascota(Mascota mascota);

        // Método para actualizar una mascota
        [OperationContract]
        [WebInvoke(Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "actualizarmascota")]
        bool ActualizarMascota(Mascota mascota);

        // Método para eliminar una mascota
        [OperationContract]
        [WebInvoke(Method = "DELETE", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "eliminarmascota")]
        bool EliminarMascota(Mascota mascota);

        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
        void HandleOptionsRequest();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "obtenermascotasfiltro", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<Mascota> ObtenerMascotasFiltro(FiltroMascotas filtro);


        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "obtenermascotasfiltrofecha")]
        List<Mascota> ObtenerMascotasFiltroFecha(StringFechaFiltro filtro);

    }
    [DataContract]
    public class ResultadoOperacion
    {
        [DataMember]
        public bool Exito { get; set; }

        [DataMember]
        public string Mensaje { get; set; }
    }

    public class FiltroMascotas
    {
        public DateTime? Fecha { get; set; }
    }

    [DataContract]
    public class StringFechaFiltro
    {
        [DataMember]
        public string Fecha { get; set; }
    }
}
