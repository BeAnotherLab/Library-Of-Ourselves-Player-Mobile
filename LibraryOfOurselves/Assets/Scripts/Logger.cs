//#define LOGGING_ENABLED // Haze-Dist

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haze {
	public static class Logger {

#if LOGGING_ENABLED

		public static void Log(object t) {
			Debug.Log(t);
		}

		public static void Log(object t, Object s) {
			Debug.Log(t, s);
		}

		public static void LogWarning(object t) {
			Debug.LogWarning(t);
		}

		public static void LogWarning(object t, Object s) {
			Debug.LogWarning(t, s);
		}

		public static void LogError(object t) {
			Debug.LogError(t);
		}

		public static void LogError(object t, Object s) {
			Debug.LogError(t, s);
		}

#else

		public static void Log(object t) {}

		public static void Log(object t, Object s) {}

		public static void LogWarning(object t) {}

		public static void LogWarning(object t, Object s) {}

		public static void LogError(object t) {}

		public static void LogError(object t, Object s) {}

#endif

	}
}