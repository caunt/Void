using System;

namespace Void.IntegrationTests.Infrastructure.Exceptions;

public class IntegrationTestException(string? message = null, Exception? innerException = null) : Exception(message, innerException);
