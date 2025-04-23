using DotNetGraph.Compilation;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using DryIoc;

namespace Void.Proxy.Utils;

public static class DryIocTracker
{
    private static readonly Lock _lock = new();
    private static readonly HashSet<TrackedContainer> _containers = [];

    public static async ValueTask<string> ToGraphStringAsync()
    {
        List<TrackedContainer> snapshot;

        using (_lock.EnterScope())
            snapshot = [.. _containers];

        var nodes = new Dictionary<IContainer, DotNode>(ReferenceEqualityComparer.Instance);
        var graph = new DotGraph()
            .WithIdentifier("Containers")
            .WithLabel("Containers")
            .WithRankDir(DotRankDir.LR)
            .Directed();

        DotNode GetOrCreateNode(TrackedContainer tracked)
        {
            if (nodes.TryGetValue(tracked.Container, out var node))
                return node;

            node = new DotNode()
                .WithIdentifier(tracked.ContainerHashCode.ToString())
                .WithLabel(tracked.Name)
                .WithShape(DotNodeShape.Ellipse)
                .WithColor(DotColor.AliceBlue)
                .WithFillColor(DotColor.AliceBlue)
                .WithStyle(DotNodeStyle.Filled);

            nodes[tracked.Container] = node;
            graph.Add(node);

            return node;
        }

        void WalkContainer(TrackedContainer parent)
        {
            var parentNode = GetOrCreateNode(parent);

            foreach (var child in parent.Childs)
            {
                var childNode = GetOrCreateNode(child);
                var edge = new DotEdge()
                    .From(parentNode)
                    .To(childNode);

                graph.Add(edge);
                WalkContainer(child);
            }
        }

        foreach (var root in snapshot)
            WalkContainer(root);

        await using var writer = new StringWriter();
        await graph.CompileAsync(new CompilationContext(writer, new CompilationOptions()));
        return writer.ToString();
    }

    public static void Track(this IContainer container, string name, IContainer? parent = null)
    {
        using (_lock.EnterScope())
        {
            var trackedAsRoot = _containers.FirstOrDefault(trackedContainer => trackedContainer.Container == container);
            var trackedParent = _containers.FirstOrDefault(trackedContainer => trackedContainer.Childs.Any(trackedChild => trackedChild.Container == container));

            if (trackedAsRoot is not null)
            {
                if (trackedAsRoot.Name == name)
                    return;

                throw new InvalidOperationException($"Container {container} is already tracked as root with name {trackedAsRoot.Name}. Cannot change name to {name}.");
            }

            if (trackedParent is not null)
            {
                var sameChild = trackedParent.Childs.First(trackedChild => trackedChild.Container == container);

                if (sameChild.Name == name)
                    return;

                throw new InvalidOperationException($"Container {container} is already tracked as child of {trackedParent.Container} with name {sameChild.Name}. Cannot change name to {name}.");
            }

            if (parent is null)
            {
                _containers.Add(new(name, container, []));
                return;
            }

            var parentTracked = _containers.FirstOrDefault(trackedContainer => trackedContainer.Container == parent) ??
                throw new InvalidOperationException($"Parent container {parent} is not tracked. Cannot track {container} as child of {parent}.");

            parentTracked.Childs.Add(new(name, container, []));
        }
    }

    public static void Untrack(this IContainer container)
    {
        using (_lock.EnterScope())
        {
            var root = _containers.FirstOrDefault(trackedContainer => trackedContainer.Container == container);

            if (root is not null)
            {
                _containers.Remove(root);
                return;
            }

            var parent = _containers.FirstOrDefault(trackedContainer => trackedContainer.Childs.Any(trackedChild => trackedChild.Container == container));

            if (parent is null)
                return;

            parent.Childs.RemoveAll(trackedContainer => trackedContainer.Container == container);
        }
    }

    private record TrackedContainer(string Name, IContainer Container, List<TrackedContainer> Childs)
    {
        public int ContainerHashCode => Container.GetHashCode();
        public override string ToString() => $"{Name} ({Container.GetHashCode()})";
    }
}
