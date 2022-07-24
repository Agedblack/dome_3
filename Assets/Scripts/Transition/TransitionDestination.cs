using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulLike
{
    public class TransitionDestination : MonoBehaviour
    {
        public enum DestinationTag
        {
            ENTER, A, B, C
        }
        public DestinationTag destinationTag;
    }
}
