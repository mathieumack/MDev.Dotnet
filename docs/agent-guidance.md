# Agent workflow

1. Inspect the target project's `TargetFramework`, existing `PackageReference` items, host type, configuration sources, and deployed Azure resources.
2. Select only the package whose capability matches the requested change. Do not add an Azure package for unrelated application concerns.
3. Reuse the solution's existing dependency injection, options, logging, credential, and infrastructure patterns.
4. Ask for only missing project-specific values such as resource names, endpoints, queue names, route prefixes, or telemetry destinations. Never ask a user to paste a credential into source code.
5. Apply the smallest idiomatic change, validate configuration during startup where the package supports it, and test both local and deployed behavior.

Use the package version already selected by the solution. For a new installation, use the latest compatible stable version unless the user explicitly requests a preview. The examples below show public APIs and omit package versions so the consuming solution can manage versions centrally.

## Security and authentication

Prefer `DefaultAzureCredential` for applications that run locally and on Azure. Use `ManagedIdentityCredential` where a workload intentionally supports only Azure managed identity, as with the Container Apps Jobs helper. Assign least-privileged data-plane roles; management-plane contributor roles do not generally grant access to service data.

Keep tokens and connection strings in local user secrets or a secure development credential store, and in the deployment platform's secret store for hosted workloads. Reference secrets from configuration or environment variables. Never commit secret values, emit them in logs, place them in an image, or render them into infrastructure outputs.

## Environment variables and secrets

| Variable | Required | Secret | Purpose |
| --- | --- | --- | --- |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | When a Container Apps Job enables Azure Monitor | Yes | Azure Monitor exporter connection string |
| `APP_API_TOKEN` | When `[RequireDaprApiToken]` protects a callback | Yes | Shared Dapr API token; callers send it as `dapr-api-token` |
| `OTEL_ENDPOINT_AUTH` | Only when the chosen OTLP endpoint requires authorization | Yes | Authorization value sent to the OTLP collector |
| `OTEL_ENDPOINT` | For Container Apps custom OTLP export | No | Base HTTP endpoint for the OTLP collector |
| `AZURE_CLIENT_ID` | For a user-assigned Container Apps Job identity | No | Managed identity client ID; omit for system-assigned identity |
| `CONTAINER_APP_NAME` | For Container Apps telemetry service naming | No | Container App name, normally supplied by the platform |
| `CONTAINER_APP_REPLICA_NAME` | No | No | Container App replica name, normally supplied by the platform |
| `CONTAINER_APP_JOB_NAME` | When job `ServiceName` is unset | No | Container Apps Job telemetry service name |

.NET environment-variable configuration can represent nested settings with `__`, for example `CosmosDb__Endpoint`. Whether such a value is secret depends on the setting; endpoints and resource names are not credentials.

## Infrastructure and deployment

MDev.Dotnet does not provision infrastructure. Before changing application code, identify whether the solution uses [Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/), [Terraform for Azure](https://learn.microsoft.com/en-us/azure/developer/terraform/), or [ARM templates](https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/) and update that existing source when resources or role assignments are missing.

- Provision the selected Azure service, its diagnostic destination, and any required containers, queues, databases, Dapr components, or Container Apps configuration.
- Enable a system-assigned identity or attach a user-assigned identity, then create least-privileged data-plane role assignments.
- Store secret settings in the platform secret store and expose only secret references to the workload.
- Permit outbound HTTPS to Azure service endpoints and telemetry collectors. Configure private endpoints, DNS, firewall rules, and virtual-network integration when public network access is disabled.
- For Dapr capabilities, enable the sidecar and configure the required component, subscription, route, and API token consistently.
- For short-lived jobs, allow enough termination time for cancellation and the configured telemetry flush timeout.

## Observability

Choose one telemetry path: Azure Monitor with its connection string, or OTLP with an endpoint and optional authorization. Keep service, `ActivitySource`, and `Meter` names consistent. Avoid logging configuration or authentication values. Verify logs, traces, and metrics in the destination after deployment; Container Apps Jobs flush all three signals during graceful host shutdown.

## Troubleshooting and testing

- Failures during registration usually indicate a missing required section, unknown property, malformed URI or connection string, or missing service name.
- HTTP 401/403 responses from Azure services usually indicate credential selection or data-plane role assignment problems; role propagation can take time.
- Managed identity endpoints are unavailable during normal local execution. Use developer credentials locally unless the package intentionally requires managed identity.
- Missing Container Apps principal headers indicate that built-in authentication or ingress protection is not configured; never trust client-supplied copies of those headers.
- Missing telemetry usually indicates an incorrect exporter choice, endpoint, connection string, source name, network rule, or abrupt process exit.
- Exercise startup validation, dependency injection resolution, and one representative service operation. Run the solution's existing build and tests rather than introducing a new test framework.

## Compatibility and deeper documentation

The packages target .NET 10. Match the source version recorded at the top of this skill with the package version used by the consuming project. Before upgrading, review the repository history and package release metadata for breaking changes, then build and test the complete solution.

This generated skill embeds the maintained package guides for agent use. Follow the links in each section for Microsoft platform documentation and browse the [human-readable documentation](https://mathieumack.github.io/MDev.Dotnet/) for deeper guidance.
