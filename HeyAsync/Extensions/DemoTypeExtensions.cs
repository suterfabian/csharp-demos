using HeyAsync.Models;

namespace HeyAsync.Extensions;

public static class DemoTypeExtensions
{
    public static string DisplayName(this DemoType demoType)
    {
        return demoType switch
        {
            DemoType.Async => "Async Programming",
            DemoType.Rx => "Rx Reactive Programming",
            _ => demoType.ToString()
        };
    }
}