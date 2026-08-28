namespace Downfall.TestCode;

public sealed record CardTestCase(string Name, Func<TestContext, Task> Run);