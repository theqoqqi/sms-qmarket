using System;
using System.Reflection;

namespace QMarketPlugin.Utils;

public static class ReflectionUtils {

    const BindingFlags instanceNonPublicFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    public static MethodInfo GetNonPublicMethod(object instance, string methodName, params Type[] types) {
        return instance.GetType().GetMethod(methodName, instanceNonPublicFlags, null, types, null);
    }

    public static T GetNonPublicFieldValue<T>(object instance, string fieldName) {
        var field = GetNonPublicField(instance, fieldName);

        return (T) (field?.GetValue(instance) ?? default(T));
    }

    public static void SetNonPublicFieldValue(object instance, string fieldName, object value) {
        var field = GetNonPublicField(instance, fieldName);

        field.SetValue(instance, value);
    }

    public static FieldInfo GetNonPublicField(object instance, string fieldName) {
        return instance.GetType().GetField(fieldName, instanceNonPublicFlags);
    }
}
