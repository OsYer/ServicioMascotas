using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System;

namespace ServicioMascotas
{
    // Inspector de mensajes para agregar encabezados CORS a las respuestas del servicio
    public class CorsEnabledMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            // Verificamos si es una solicitud OPTIONS
            var httpRequestProperty = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;

            if (httpRequestProperty != null && httpRequestProperty.Method == "OPTIONS")
            {
                // Si es una solicitud OPTIONS, no necesitamos hacer nada con el contenido.
                // Simplemente pasamos para agregar los encabezados CORS en la respuesta.
                return null; // Pasamos la solicitud OPTIONS sin modificarla
            }

            return null; // Si no es OPTIONS, continuamos normalmente
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply == null) return;

            // Agregar los encabezados CORS a la respuesta
            var httpHeader = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
            if (httpHeader == null)
            {
                httpHeader = new HttpResponseMessageProperty();
                reply.Properties.Add(HttpResponseMessageProperty.Name, httpHeader);
            }

            // Agregar encabezados CORS para la respuesta
            httpHeader.Headers.Set("Access-Control-Allow-Origin", "*"); // Permitir todos los orígenes
            httpHeader.Headers.Set("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS"); // Métodos permitidos
            httpHeader.Headers.Set("Access-Control-Allow-Headers", "Content-Type, Authorization"); // Encabezados permitidos

            // Si es una solicitud OPTIONS, no enviamos contenido en la respuesta, solo los encabezados
            if (httpHeader.Headers["Access-Control-Allow-Origin"] == "*")
            {
                // Responder con estado 200 OK para las solicitudes OPTIONS
                reply = Message.CreateMessage(reply.Version, reply.Headers.Action, new HttpResponseMessageProperty { StatusCode = HttpStatusCode.OK });
            }
        }
    }

    // Comportamiento para aplicar CORS a todos los puntos finales
    public class CorsEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // Registrar el inspector para agregar los encabezados CORS a las respuestas
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CorsEnabledMessageInspector());
        }

        public void Validate(ServiceEndpoint endpoint) { }
    }

    // Extensión de comportamiento para configurar CORS desde el archivo web.config
    public class CorsBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType => typeof(CorsEndpointBehavior);

        protected override object CreateBehavior()
        {
            return new CorsEndpointBehavior();
        }
    }
}
