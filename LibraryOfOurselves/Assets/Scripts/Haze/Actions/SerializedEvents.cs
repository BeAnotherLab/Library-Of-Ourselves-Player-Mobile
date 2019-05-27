using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class FloatEvent : UnityEvent<float>{}
[Serializable] public class IntEvent : UnityEvent<int>{}
[Serializable] public class StringEvent : UnityEvent<string>{}
[Serializable] public class Vector2Event : UnityEvent<Vector2>{}
[Serializable] public class Vector3Event : UnityEvent<Vector3>{}
[Serializable] public class ColorEvent : UnityEvent<Color>{}
[Serializable] public class StringStringEvent : UnityEvent<string, string>{}
[Serializable] public class StringColorEvent : UnityEvent<string, Color>{}
[Serializable] public class ListOfBytesIntEvent : UnityEvent<List<byte>, int>{}
