using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.SharePoint.Utilities;

namespace Chuchuka.SharePoint.Utilities.Utilities.ServiceBehaviours
{
	public class ValidateFormDigestAttribute : Attribute, IServiceBehavior
	{
		public void Validate(ServiceDescription description, ServiceHostBase host)
		{
		}

		public void AddBindingParameters(ServiceDescription description, ServiceHostBase host, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase host)
		{
			foreach (var channelDispatcherBase in host.ChannelDispatchers)
			{
				var dispatcher = (ChannelDispatcher)channelDispatcherBase;
				foreach (EndpointDispatcher eDispatcher in dispatcher.Endpoints)
				{
					eDispatcher.DispatchRuntime.MessageInspectors.Add(new ValidateSPFormDigestInspector());
				}
			}
		}

		internal class ValidateSPFormDigestInspector : IDispatchMessageInspector
		{
			private const string Message = "The security validation for this page is invalid. Click Back in your Web browser, refresh the page, and try your operation again.";

			public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
			{
				var httpRequest = request.Properties["httpRequest"] as HttpRequestMessageProperty;
				if (httpRequest != null && httpRequest.Method == "POST" && !SPUtility.ValidateFormDigest())
				{
					throw new FaultException(new FaultReason(Message));
				}
				return null;
			}

			public void BeforeSendReply(ref Message reply, object correlationState)
			{
			}
		}
	}
}
