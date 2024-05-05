using System;
using System.Reflection;

namespace QMarketPlugin.Utils;

public static class ReflectionUtils {

    const BindingFlags instanceNonPublicFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    public static MethodInfo GetNonPublicMethod(object instance, string methodName, params Type[] types) {
        return instance.GetType().GetMethod(methodName, instanceNonPublicFlags, null, types, null);
    }
}
