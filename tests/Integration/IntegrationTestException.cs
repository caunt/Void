using System;

namespace Void.Tests.Integration;

public class IntegrationTestException(string? message = null, Exception? innerException = null) : Exception(message, innerException);
