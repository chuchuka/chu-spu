using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chuchuka.SharePoint.Utilities.Utilities.ServiceBehaviours
{
	public class JsonErrorHandler : IErrorHandler
	{
		/// <summary>
		/// Handle all exceptions
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public bool HandleError(Exception exception)
		{
			return true;
		}

		public void ProvideFault(Exception exception, MessageVersion version, ref Message fault)
		{
			fault = GetJsonFaultMessage(version, exception);

			var jsonFormatting = new WebBodyFormatMessageProperty(WebContentFormat.Json);
			fault.Properties.Add(WebBodyFormatMessageProperty.Name, jsonFormatting);

			var httpResponse = new HttpResponseMessageProperty
			{
				StatusCode = HttpStatusCode.BadRequest,
				StatusDescription = "Bad Request"
			};

			fault.Properties.Add(HttpResponseMessageProperty.Name, httpResponse);
		}

		protected virtual Message GetJsonFaultMessage(MessageVersion version, Exception exception)
		{
			var fault = new JsonFault
			{
				Type = exception.GetType().Name,
				Message = exception.Message
			};

			var serializer = new DataContractJsonSerializer(fault.GetType(), new List<Type> { typeof(JsonFault) });
			return Message.CreateMessage(version, "exception", fault, serializer);
		}
	}
}
