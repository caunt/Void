namespace Void.Terminal;

public delegate ValueTask<string[]> Autocompletion(string input, CancellationToken cancellationToken);
