# Void Plugin Development Kit (PDK)

This directory contains a sample plugin project that demonstrates how to develop plugins for Void Proxy.

## Getting Started

### Download the PDK

The Plugin Development Kit is available as a downloadable package from the [**latest release**](https://github.com/caunt/Void/releases/latest/download/plugin-devkit.zip).

### Project Structure

- `YourPlugin/` - Sample plugin project demonstrating the plugin API
- `pdk.slnx` - Solution file for the plugin development kit

### Documentation

For complete plugin development documentation, see:
- [**Plugin Development Kit**](https://void.caunt.world/docs/developing-plugins/development-kit/)
- [**Example Plugin**](https://github.com/caunt/Void/blob/main/src/Plugins/ExamplePlugin/ExamplePlugin.cs)
- [**Commands**](https://void.caunt.world/docs/developing-plugins/commands/)
- [**Events**](https://void.caunt.world/docs/developing-plugins/events/listening-to-events/)
- [**Network Packets**](https://void.caunt.world/docs/developing-plugins/network/packets/)

## Building Your Plugin

1. Open the solution in your IDE (Visual Studio, Rider, or VS Code)
2. Rename `YourPlugin` to your plugin name
3. Implement your plugin logic in the `.cs` file
4. Build the project to generate a `.dll` file
5. Load the plugin using the `--plugin` argument or configuration file

## Learn More

- [**Main Documentation**](https://void.caunt.world/docs/)
- [**Repository**](https://github.com/caunt/Void)
