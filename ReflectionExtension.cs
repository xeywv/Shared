using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class ReflectionExtension
    {
        static public void ForceTimeOut<T>(T  thread, string timerName)
        {
            object timer      = thread.GetType().GetField(timerName, BindingFlags.NonPublic|BindingFlags.Instance).GetValue(thread);
            object stopwatch  = timer.GetType().GetField("stopwatch", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(timer);
            ulong interval    = (ulong)timer.GetType().GetProperty("Interval", BindingFlags.Public|BindingFlags.Instance).GetValue(timer, new object[0]);

            stopwatch.GetType().GetField("startTimeStamp", BindingFlags.NonPublic|BindingFlags.Instance).SetValue(stopwatch, DateTime.Now.AddMilliseconds(interval).TimeOfDay.Ticks);
        }

        static public void CallPrivateMethod<T>(T obj, string methodName)
        {
            MethodInfo methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic|BindingFlags.Instance);
            methodInfo.Invoke(obj, new object[0]);
        }

        static public void CallPrivateStaticMethod<T>(string methodName, params object[] parameters)
        {
            MethodInfo methodInfo = typeof(T).GetMethod(methodName, BindingFlags.NonPublic|BindingFlags.Static);
            methodInfo.Invoke(null, parameters);
        }

        static public R CallPrivateStaticMethod<T,R>(string methodName, params object[] parameters)
        {
            MethodInfo methodInfo = typeof(T).GetMethod(methodName, BindingFlags.NonPublic|BindingFlags.Static);
            return (R)methodInfo.Invoke(null, parameters);
        }
    }
}
