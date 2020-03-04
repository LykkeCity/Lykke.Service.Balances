using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Lykke.Service.Balances.Services
{
    public class TelemetryHelper
    {
        private static readonly TelemetryClient Telemetry = new TelemetryClient();

        public static IOperationHolder<RequestTelemetry> InitTelemetryOperation(
            string name,
            string id)
        {

            string operationId = Guid.Parse(id).ToString("N");
            var requestTelemetry = new RequestTelemetry { Name = name };

            requestTelemetry.Id = operationId;
            requestTelemetry.Context.Operation.Id = operationId;

            return Telemetry.StartOperation(requestTelemetry);
        }

        public static void SubmitException(IOperationHolder<RequestTelemetry> telemtryOperation, Exception e)
        {
            telemtryOperation.Telemetry.Success = false;
            Telemetry.TrackException(e);
        }

        public static void SubmitOperationResult(IOperationHolder<RequestTelemetry> telemtryOperation)
        {
            Telemetry.StopOperation(telemtryOperation);
        }
    }
}
