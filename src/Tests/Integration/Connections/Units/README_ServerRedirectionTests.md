# Multi-Server Redirection Integration Tests

This document describes the newly added integration tests for multi-server redirection functionality in the Void proxy.

## Overview

The `ServerRedirectionTests` class provides comprehensive integration tests that verify the proxy's ability to:

1. Set up multiple backend servers
2. Route clients between servers using the `/server` command
3. Handle both MinecraftConsoleClient and MineflayerClient redirection
4. Gracefully handle invalid server names

## Test Structure

### Components Setup
- **2 PaperServers**: `PaperServer1` (port 36001) and `PaperServer2` (port 36002)
- **1 VoidProxy**: Configured with both servers on port 36000
- **2 Client Types**: MinecraftConsoleClient and MineflayerClient

### Test Cases

1. **BasicMultiServerSetup_ConnectsToFirstServer**: Verifies basic connectivity to the first server
2. **MccRedirectsBetweenServers**: Full redirection test using MinecraftConsoleClient
3. **MineflayerRedirectsBetweenServers**: Full redirection test using MineflayerClient  
4. **Protocol Version Tests**: Tests redirection with various Minecraft protocol versions
5. **Invalid Server Name Test**: Verifies handling of non-existent server names

## Key Features

### Enhanced Components

1. **PaperServer**: Modified to accept a `name` parameter to avoid subfolder conflicts
   ```csharp
   PaperServer.CreateAsync(workingDirectory, client, port: 36001, name: "PaperServer1")
   ```

2. **VoidProxy**: Enhanced to support multiple servers
   ```csharp
   VoidProxy.CreateAsync(new[] { "localhost:36001", "localhost:36002" }, proxyPort: 36000)
   ```

3. **Clients**: Extended with `SendCommandAsync` for sending commands with custom timeouts
   ```csharp
   client.SendCommandAsync(address, protocolVersion, "/server args-server-2", TimeSpan.FromSeconds(10))
   ```

## Running the Tests

### Environment Variables
Tests require the following environment variables to be set:
- `VOID_INTEGRATION_PROXIED_TESTS_ENABLED=true`

### Command
```bash
dotnet test --filter "Category=ProxiedFact|Category=ProxiedTheory"
```

### Expected Server Names
The proxy automatically names command-line specified servers as:
- First server: `args-server-1`
- Second server: `args-server-2`

These names are used in the `/server` commands for redirection.

## Test Flow Example

1. Client connects to proxy (routes to `args-server-1` by default)
2. Client sends message → verified on server1
3. Client sends `/server args-server-2` command → proxy redirects to server2
4. Client sends message → verified on server2
5. Client sends `/server args-server-1` command → proxy redirects back to server1
6. Client sends message → verified on server1

## Notes

- Tests have extended timeout (5 minutes) due to complexity of multi-server operations
- Both client types (MCC and Mineflayer) are tested to ensure broad compatibility
- Edge cases like invalid server names are tested to ensure robustness
- Protocol version compatibility is verified across supported Minecraft versions