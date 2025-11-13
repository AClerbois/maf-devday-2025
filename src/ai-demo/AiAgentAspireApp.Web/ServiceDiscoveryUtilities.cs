public static class ServiceDiscoveryUtilities
{
    public static string? GetServiceEndpoint(string serviceName, string endpointName, int index = 0) =>
      Environment.GetEnvironmentVariable($"services__{serviceName}__{endpointName}__{index}");
}
