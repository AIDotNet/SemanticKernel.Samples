using System.Reflection;

namespace Samples4;

/// <summary>
/// 提供从程序集中读取嵌入资源的功能。
/// 此类设计用于访问嵌入在同一程序集中的基于文本的资源。
/// 它根据名称检索指定资源文件的内容，并将其作为字符串返回。
/// </summary>
public static class EmbeddedResource
{
    private static readonly string? s_namespace = typeof(EmbeddedResource).Namespace;

    internal static string Read(string fileName)
    {
        // Get the current assembly. Note: this class is in the same assembly where the embedded resources are stored.
        Assembly assembly =
            typeof(EmbeddedResource).GetTypeInfo().Assembly ??
            throw new InvalidOperationException($"[{s_namespace}] {fileName} assembly not found");

        // Resources are mapped like types, using the namespace and appending "." (dot) and the file name
        var resourceName = $"{s_namespace}." + fileName;
        using Stream resource =
            assembly.GetManifestResourceStream(resourceName) ??
            throw new InvalidOperationException($"{resourceName} resource not found");

        // Return the resource content, in text format.
        using var reader = new StreamReader(resource);

        return reader.ReadToEnd();
    }
}
