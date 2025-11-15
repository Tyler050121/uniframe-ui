using System;
using UnityEngine;

namespace UIFramework.ViewAnimation
{
    public abstract class AniComponent : MonoBehaviour
    {
        public abstract void Animate(Transform target, Action onComplete);
    }
}