---
title: Development Kit
description: Learn how to use the Plugin Development Kit to create plugins for Void.
---

Plugin Development Kit is a set of predefined configurations that simplify the development process of plugins.
It can be used with any MSBuild-compatible IDE, such as Visual Studio or JetBrains Rider.

## Prerequisites
Void plugins are written with many modern .NET features, so you will need to ensure that you have experience with them or at least know what you are doing.
Some of them include:
- [**Dependency Injection**](/docs/developing-plugins/services/creating-a-service/)
- Asynchronous Programming
- [**Event-Driven Programming**](/docs/developing-plugins/events/listening-to-events/)
- Stack-allocating and Memory Management
- [**Serialization and Deserialization of data**](/docs/developing-plugins/serializers/)
- [**Network Packets**](/docs/developing-plugins/network/packets/) and Protocols

## Installation
1) [**Download**](https://github.com/caunt/Void/releases/latest/download/plugin-devkit.zip) the latest **Plugin Development Kit**.
2) Extract the downloaded zip file to your desired location.
3) Open the **pdk.slnx** file with your IDE.

## Running
Press **F5** to run the project. This will start the Void Proxy with your plugin loaded.

## Debugging
:::caution
Debugging is currently not supported by JetBrains Rider.
Proxy is distributed with the `PublishSingleFile=true` flag, which makes Rider unable to attach the debugger to the process.
See [**Rider Debugging**](https://www.jetbrains.com/help/rider/Debugging_Code.html) for more details.
:::

With Visual Studio, you can set breakpoints in your code and use the **Debug** menu to start debugging.

## Compiling
1) Build the project with your IDE.
2) Take the compiled DLL from the **bin** folder.
3) Do not use and do not include any other DLLs from that directory.
Your dependencies will be automatically resolved by the proxy at runtime.

## Dependencies
Dependencies are automatically resolved at runtime using several fallback mechanisms.
This includes NuGet packages, local DLLs, and runtime lookups.
NuGet dependencies will be automatically downloaded and cached in the **packages** directory.

## Distribution
Share your **.dll** file, without any other files from the **bin** directory.
