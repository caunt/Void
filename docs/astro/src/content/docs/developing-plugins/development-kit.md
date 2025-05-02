---
title: Development Kit
description: Learn how to use the Plugin Development Kit to create plugins for Void
---

Plugin Development Kit is a set of predefined configurations that simplifies the development process of plugins.
It can be used with any msbuild-compatible IDE, such as Visual Studio or Rider.

## Prerequisites
Void plugins are written with many modern .NET features, so you will need to ensure that you had experience with them or at least know what are you doing.
Some of them include:
- Dependency Injection
- Asynchronous Programming
- Event-Driven Programming
- Stack-allocating and Memory Management
- Serialization and Deserialization of data
- Network Packets and Protocols

## Installation
1) [**Download**](https://github.com/caunt/Void/releases/latest/download/plugin-devkit.zip) the latest **Plugin Development Kit**.
2) Extract the downloaded zip file to your desired location.
3) Open the ***.slnx** file with your IDE.

## Running
Press **F5** to run the project. This will start the Void Proxy with your plugin loaded.

## Debugging
:::caution
Debugging currently is not supported by JetBrains Rider.
Proxy is distributed with PublishSingleFile=true flag, which makes Rider unable to attach debugger to the process.
See [Rider Debugging](https://www.jetbrains.com/help/rider/Debugging_Code.html) for more details.
:::

With Visual Studio, you can set breakpoints in your code and use the **Debug** menu to start debugging.

## Compiling
1) Build the project with your IDE.
2) Take the compiled dll from the **bin** folder.
3) Do not use and do not include any other dlls from that directory. 
Your dependencies will be automatically resolved by the proxy in runtime.

## Dependencies
Dependencies are automatically resolved in runtime in several ways and fallbacks. 
Including nuget packages, local dlls, and looking in runtime.
NuGet dependencies will be automatically downloaded and cached in the **packages** directory.

## Distribution
Share your ***.dll** file, without any other files from the **bin** directory.