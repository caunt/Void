---
title: Running
description: Learn how to run Void Proxy
---

[**Download**](https://github.com/caunt/Void/releases/latest/) latest release

**Supported platforms**:

| OS - Arch       | X64 | ARM64 | ARM | X86 |
|:---------------:|:---:|:-----:|:---:|:---:|
| Linux           | ✅ | ✅ | ✅ | ❌ |
| Linux (Alpine)  | ✅ | ✅ | ✅ | ❌ |
| Linux (Android) | ✅ | ✅ | ❌ | ❌ |
| macOS           | ✅ | ✅ | ❌ | ❌ |
| Windows         | ✅ | ✅ | ❌ | ✅ |

### Linux

Set the executable permission
```bash
chmod +x void-linux-x64
```

Run the executable
```bash
./void-linux-x64 [optional arguments]
```

### Android

Run the [**Termux**](https://play.google.com/store/apps/details?id=com.termux) or any other terminal emulator.

Follow instructions for Linux, but use "bionic" version of binary.

### Alpine

Follow instructions for Linux, but use "musl" version of binary.

### macOS

Follow instructions for Linux, but use "osx" version of binary.

### Windows

Run the executable, simple as that.