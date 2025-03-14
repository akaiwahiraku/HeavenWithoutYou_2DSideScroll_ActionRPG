// Copyright (c) Pixel Crushers. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers
{

    // This file defines common UnityEvent types.

    [Serializable]
    public class IntUnityEvent : UnityEvent<int> { }

    [Serializable]
    public class StringUnityEvent : UnityEvent<string> { }

    [Serializable]
    public class ObjectUnityEvent : UnityEvent<object> { }

    [Serializable]
    public class GameObjectUnityEvent : UnityEvent<GameObject> { }

}
