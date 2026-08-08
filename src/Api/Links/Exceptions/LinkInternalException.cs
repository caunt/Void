namespace Void.Proxy.Api.Links.Exceptions;

/// <summary>
/// Represents an invariant or processing failure internal to a link implementation.
/// </summary>
/// <param name="message">An optional message describing the failure.</param>
/// <param name="innerException">The exception that caused this failure, or <see langword="null" /> when no underlying exception is available.</param>
public class LinkInternalException(string? message = null, Exception? innerException = null) : Exception(message, innerException);
