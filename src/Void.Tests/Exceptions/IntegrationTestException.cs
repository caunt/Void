using System;

namespace Void.Tests.Exceptions;

public class IntegrationTestException(string? message = null, Exception? innerException = null) : Exception(message, innerException);
