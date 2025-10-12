using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Learnings.Api
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
        public string RoleName { get; set; } = "learnings-api";
        public string EnvironmentName { get; set; } = "Development";

        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                telemetry.Context.Cloud.RoleName = RoleName;
            }

            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleInstance))
            {
                telemetry.Context.Cloud.RoleInstance = EnvironmentName;
            }

            // Add custom properties
            telemetry.Context.GlobalProperties["Environment"] = EnvironmentName;
            telemetry.Context.GlobalProperties["Application"] = RoleName;
        }
    }
}