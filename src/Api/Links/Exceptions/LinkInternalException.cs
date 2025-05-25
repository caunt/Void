namespace Void.Proxy.Api.Links.Exceptions;

public class LinkInternalException(string? message = null, Exception? innerException = null) : Exception(message, innerException);
