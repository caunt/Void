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

Gets a successful authentication result with a default human-readable message.

```csharp
public static AuthenticationResult Authenticated { get; }
```

#### Property Value

 [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

#### Examples

<pre><code class="lang-csharp">var result = AuthenticationResult.Authenticated;
if (result.IsAuthenticated)
{
    // Continue login flow.
}</code></pre>

#### Remarks

<p>
Each access creates a new record instance instead of returning a cached object.
</p>
<p>
Use this value for successful authentication branches that should compare by record value semantics
(for example, checks against <xref href="Void.Proxy.Api.Events.Authentication.AuthenticationResult.NoResult" data-throw-if-not-resolved="false"></xref> or other predefined outcomes).
</p>

#### See Also

[AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md).[AlreadyAuthenticated](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md\#Void\_Proxy\_Api\_Events\_Authentication\_AuthenticationResult\_AlreadyAuthenticated), 
[AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md).[NoResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md\#Void\_Proxy\_Api\_Events\_Authentication\_AuthenticationResult\_NoResult)

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

Gets a sentinel authentication result that indicates no final outcome has been produced yet.

```csharp
public static AuthenticationResult NoResult { get; }
```

#### Property Value

 [AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md)

#### Examples

<pre><code class="lang-csharp">if (result == AuthenticationResult.NoResult)
    continue;</code></pre>

#### Remarks

<p>
This value is used as a control signal while processing server login packets: handlers return it to continue waiting for more packets.
</p>
<p>
Equality checks against <xref href="Void.Proxy.Api.Events.Authentication.AuthenticationResult.NoResult" data-throw-if-not-resolved="false"></xref> are value-based because <xref href="Void.Proxy.Api.Events.Authentication.AuthenticationResult" data-throw-if-not-resolved="false"></xref> is a record, even though each access creates a new instance.
</p>

#### See Also

[AuthenticationResult](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md).[Authenticated](Void.Proxy.Api.Events.Authentication.AuthenticationResult.md\#Void\_Proxy\_Api\_Events\_Authentication\_AuthenticationResult\_Authenticated)

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

