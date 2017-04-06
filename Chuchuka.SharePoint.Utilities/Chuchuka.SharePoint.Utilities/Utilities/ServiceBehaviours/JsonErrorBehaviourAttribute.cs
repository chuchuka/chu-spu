using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Chuchuka.SharePoint.Utilities.Utilities.ServiceBehaviours
{
	public class JsonErrorBehaviourAttribute : Attribute, IServiceBehavior
	{
		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
			{
				endpoint.EndpointBehaviors.Add(new JsonErrorWebHttpBehaviour());
			}
		}
	}
}
