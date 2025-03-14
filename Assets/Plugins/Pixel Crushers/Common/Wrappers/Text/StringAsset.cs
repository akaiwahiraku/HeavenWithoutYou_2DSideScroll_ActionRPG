﻿// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.Wrappers
{

    [CreateAssetMenu(menuName = "Pixel Crushers/Common/Text/String Asset")]
    /// <summary>
    /// This wrapper for PixelCrushers.StringAsset keeps references intact if you
    /// switch between the compiled assembly and source code versions of the original
    /// class.
    /// </summary>
    public class StringAsset : PixelCrushers.StringAsset
    {
    }

}
