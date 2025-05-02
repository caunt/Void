namespace Void.Playground;

// Bug: https://github.com/dotnet/docfx/issues/10655
// Bug: docfx docfx/docfx.json --serve

public ref struct TestStruct;

public static class TestClass
{
    public static byte TestMethod<TRefStruct>(this TRefStruct value) where TRefStruct : allows ref struct => 1;
}
