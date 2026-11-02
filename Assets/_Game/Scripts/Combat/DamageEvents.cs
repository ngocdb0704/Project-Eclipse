using System;
using UnityEngine;

namespace Eclipse.Combat
{
    public static class DamageEvents
    {
        public static event Action<DamageEventData>
            DamageDealt;

        internal static void Raise(
            DamageEventData data)
        {
            DamageDealt?.Invoke(data);
        }

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            DamageDealt = null;
        }
    }
}