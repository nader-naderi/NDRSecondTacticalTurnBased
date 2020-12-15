using UnityEngine;
using System.Collections;

namespace NDR2ndTTB
{
    public static class StaticFunctions
    {
        public static int S_NormalToTarget(EStance t)
        {
            switch (t)
            {
                default:
                case EStance.Normal:
                    return 0;
                case EStance.Run:
                case EStance.Crouch:
                    return 2;
                case EStance.Prone:
                    return 4;
            }
        }

        public static int S_CrouchToTarget(EStance t)
        {
            switch (t)
            {
                default:
                case EStance.Normal:
                    return 2;
                case EStance.Run:
                case EStance.Crouch:
                    return 0;
                case EStance.Prone:
                    return 2;
            }
        }
        public static int S_ProneToTarget(EStance t)
        {
            switch (t)
            {
                default:
                case EStance.Normal:
                    return 4;
                case EStance.Run:
                case EStance.Crouch:
                    return 2;
                case EStance.Prone:
                    return 0;
            }
        }
    }
}