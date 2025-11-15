using System;
using UnityEngine;

namespace UIFramework.ViewAnimation
{
    public class FadeAni : AniComponent
    {
        [SerializeField] float m_FadeDuration = 0.3f;
        [SerializeField] bool m_FadeOut = false;

        CanvasGroup m_CanvasGroup = null;
        float m_Timer;
        Action m_CurrentAction;
        Transform m_CurrentTarget;

        float m_StartValue;
        float m_EndValue;

        bool m_ShouldAnimate;

        public override void Animate(Transform target, Action onComplete)
        {
            if (m_CurrentAction != null)
            {
                m_CanvasGroup.alpha = m_EndValue;
                m_CurrentAction();
            }

            m_CanvasGroup = target.GetComponent<CanvasGroup>();
            if (m_CanvasGroup == null)
            {
                m_CanvasGroup = target.gameObject.AddComponent<CanvasGroup>();
            }

            if (m_FadeOut)
            {
                m_StartValue = 1f;
                m_EndValue = 0f;
            }
            else
            {
                m_StartValue = 0f;
                m_EndValue = 1f;
            }

            m_CurrentAction = onComplete;
            m_Timer = m_FadeDuration;

            m_CanvasGroup.alpha = m_StartValue;
            m_ShouldAnimate = true;
        }

        void Update()
        {
            if (!m_ShouldAnimate) return;

            m_Timer -= Time.deltaTime;
            if (m_Timer <= 0f)
            {
                m_CanvasGroup.alpha = m_EndValue;
                m_ShouldAnimate = false;
                m_CurrentAction?.Invoke();
                m_CurrentAction = null;
            }
            else
            {
                m_CanvasGroup.alpha = Mathf.Lerp(m_StartValue, m_EndValue, m_Timer / m_FadeDuration);
            }
        }
    }
}