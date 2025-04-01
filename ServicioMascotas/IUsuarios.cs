using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using ServicioMascotas.Models;

namespace ServicioMascotas
{
    [ServiceContract]
    public interface IUsuarios
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "obtenerusuarios")]
        List<Usuario> ObtenerUsuarios();

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "agregarusuario")]
        bool AgregarUsuario(Usuario usuario);

        [OperationContract]
        [WebInvoke(Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "actualizarusuario")]
        bool ActualizarUsuario(Usuario usuario);

        [OperationContract]
        [WebInvoke(Method = "DELETE", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "eliminarusuario")]
        bool EliminarUsuario(Usuario usuario);

        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
        void HandleOptionsRequest();
    }
}
