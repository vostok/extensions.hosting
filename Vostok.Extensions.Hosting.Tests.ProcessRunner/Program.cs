using System.Reflection;

Console.WriteLine($"Params: {string.Concat(args.Select(x => $@"""{x}"" "))}");
EnsureValue(args, 0, "assembly", "Assembly path is absent", out var assemblyPath);
EnsureValue(args, 1, "type", "Name of type is absent", out var typeName);
EnsureValue(args, 2, "method", "Name of method is absent", out var methodName);

var assembly = Assembly.LoadFrom(assemblyPath);
Console.WriteLine($"Assembly: {assembly}");

var type = assembly.GetType(typeName, true) ?? throw new ArgumentException($"Type '{typeName}' not found", "type"); 
Console.WriteLine($"Type: {type}");

var method = type.GetMethod(methodName) ?? throw new ArgumentException($"Method '{methodName}' not found", "method"); 
Console.WriteLine($"Method: {method}");

var instance = Activator.CreateInstance(type); 
Console.WriteLine($"Instance: {instance}");

method.Invoke(instance, new object[]{args.Skip(3).ToArray()}); 
Console.WriteLine("Done!");
return;

static void EnsureValue(IReadOnlyList<string> args, int index, string name, string message, out string value)
{
    if (args.Count <= index) throw new ArgumentOutOfRangeException(name, index, message);
    value = args[index];
}