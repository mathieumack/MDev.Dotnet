using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDev.Dotnet.AspNetCore.OpenTelemetry.Settings;

public class OpenTelemetrySettings
{
    public const string SectionName = "OpenTelemetry";

    public string ServiceType { get; set; }
}
