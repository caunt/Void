# <a id="Void_Proxy_Api_Events_Authentication_AuthenticationResult"></a> Class AuthenticationResult

Namespace: [Void.Proxy.Api.Events.Authentication](Void.Proxy.Api.Events.Authentication.md)  
Assembly: Void.Proxy.Api.dll  

```csharp
public record AuthenticationResult : IEquatable<AuthenticationResult>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

#### Implements

[IEquatable<AuthenticationResult\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationResult__ctor_System_Boolean_System_String_"></a> AuthenticationResult\(bool, string?\)

```csharp
public AuthenticationResult(bool IsAuthenticated, string? Message = null)
```

#### Parameters

`IsAuthenticated` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`Message` [string](https://learn.microsoft.com/dotnet/api/system.string)?

## Properties

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationResult_AlreadyAuthenticated"></a> AlreadyAuthenticated

```csharp
public static AuthenticationResult AlreadyAuthenticated { get; }
```

#### Property Value

 [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationResult_Authenticated"></a> Authenticated

```csharp
public static AuthenticationResult Authenticated { get; }
```

#### Property Value

 [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationResult_IsAuthenticated"></a> IsAuthenticated

```csharp
public bool IsAuthenticated { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationResult_Message"></a> Message

```csharp
public string? Message { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationResult_NoResult"></a> NoResult

```csharp
public static AuthenticationResult NoResult { get; }
```

#### Property Value

 [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationResult_NotAuthenticatedPlayer"></a> NotAuthenticatedPlayer

```csharp
public static AuthenticationResult NotAuthenticatedPlayer { get; }
```

#### Property Value

 [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

### <a id="Void_Proxy_Api_Events_Authentication_AuthenticationResult_NotAuthenticatedServer"></a> NotAuthenticatedServer

```csharp
public static AuthenticationResult NotAuthenticatedServer { get; }
```

#### Property Value

 [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

