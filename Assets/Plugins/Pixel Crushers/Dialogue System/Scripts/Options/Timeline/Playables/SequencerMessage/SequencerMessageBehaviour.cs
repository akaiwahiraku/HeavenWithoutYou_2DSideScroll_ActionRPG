#if USE_TIMELINE
#if UNITY_2017_1_OR_NEWER
// Copyright (c) Pixel Crushers. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace PixelCrushers.DialogueSystem
{

    [Serializable]
    public class SequencerMessageBehaviour : PlayableBehaviour
    {

        [Tooltip("Sequencer message to send to Dialogue System's sequencer.")]
        public string message;

    }
}
#endif
#endif
