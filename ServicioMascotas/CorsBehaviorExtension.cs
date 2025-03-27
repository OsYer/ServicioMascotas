using System;
using System.ServiceModel.Configuration;

namespace ServicioMascotas
{
    public class CorsBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType => typeof(EnableCorsBehavior);

        protected override object CreateBehavior()
        {
            return new EnableCorsBehavior();
        }
    }
}
