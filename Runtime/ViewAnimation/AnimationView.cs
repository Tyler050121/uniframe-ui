using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace UIFramework.ViewAnimation
{
    public class AnimationsView : AniComponent
    {
        [SerializeField] AnimationClip m_AnimationClip;
        [SerializeField] bool m_PlayReverse = false;

        Action m_PreviousCallbackWhenFinished;

        public override void Animate(Transform target, Action onComplete)
        {
            FinishPrevious();
            var targetAnimation = target.GetComponent<Animation>();
            if (targetAnimation == null)
            {
                Debug.LogError("[AnimationsView] Target GameObject does not have an Animation component.");
                onComplete?.Invoke();
                return;
            }

            targetAnimation.clip = m_AnimationClip;
            StartCoroutine(PlayAnimationRoutine(targetAnimation, onComplete));
        }

        IEnumerator PlayAnimationRoutine(Animation targetAnimation, Action onComplete)
        {
            m_PreviousCallbackWhenFinished = onComplete;
            foreach (AnimationState state in targetAnimation)
            {
                state.time = m_PlayReverse ? state.length : 0f;
                state.speed = m_PlayReverse ? -1f : 1f;
            }

            targetAnimation.Play(PlayMode.StopAll);
            yield return new WaitForSeconds(targetAnimation.clip.length);
            FinishPrevious();
        }

        void FinishPrevious()
        {
            m_PreviousCallbackWhenFinished?.Invoke();
            m_PreviousCallbackWhenFinished = null;
        }
    }
}