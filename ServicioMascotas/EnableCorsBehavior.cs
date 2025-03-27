using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Collections.ObjectModel;

namespace ServicioMascotas
{
    public class EnableCorsBehavior : IDispatchMessageInspector, IServiceBehavior
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply != null && reply.Properties.ContainsKey(HttpResponseMessageProperty.Name))
            {
                var httpHeader = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
                if (httpHeader != null)
                {
                    httpHeader.Headers.Add("Access-Control-Allow-Origin", "*");
                    httpHeader.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                    httpHeader.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization");
                }
            }
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher eDispatcher in cDispatcher.Endpoints)
                {
                    eDispatcher.DispatchRuntime.MessageInspectors.Add(new EnableCorsBehavior());
                }
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
