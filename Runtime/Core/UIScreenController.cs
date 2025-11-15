using System;
using UIFramework.ViewAnimation;
using UnityEngine;

namespace UIFramework.Core
{
    public abstract class UIScreenController<TProps> : MonoBehaviour, IScreenController where TProps : IScreenProperties
    {
        [Header("Screen Animations")]
        [SerializeField, Tooltip("进入界面的动画")] AniComponent m_AnimIn;
        [SerializeField, Tooltip("退出界面的动画")] AniComponent m_AnimOut;

        [Header("Screen Properties")]
        [SerializeField, Tooltip("界面的属性参数")] TProps m_Properties;

        public string ScreenID { get; set; }

        public AniComponent AnimIn => m_AnimIn;
        public AniComponent AnimOut => m_AnimOut;

        public Action<IScreenController> InTransitionFinished { get; set; }
        public Action<IScreenController> OutTransitionFinished { get; set; }
        public Action<IScreenController> CloseRequest { get; set; }
        public Action<IScreenController> ScreenDestroyed { get; set; }

        public bool IsVisible { get; protected set; }

        protected TProps Properties
        {
            get => m_Properties;
            set => m_Properties = value;
        }

        protected virtual void Awake()
        {
            AddListeners();
        }

        protected virtual void OnDestroy()
        {
            ScreenDestroyed?.Invoke(this);
            InTransitionFinished = null;
            OutTransitionFinished = null;
            CloseRequest = null;
            ScreenDestroyed = null;
            RemoveListeners();
        }

        protected virtual void AddListeners()
        {
        }

        protected virtual void RemoveListeners()
        {
        }

        protected virtual void OnPropertiesSet()
        {
        }

        protected virtual void WhileHiding()
        {
        }

        protected virtual void SetProperties(TProps props)
        {
            Properties = props;
        }

        /// <summary>
        /// 在显示的时候处理一些层级,或者属性处理等,具体看继承者重写了
        /// </summary>
        protected virtual void HierachyFixOnShow()
        {

        }

        public void Hide(bool animate = true)
        {
            DoAnimation(animate ? m_AnimOut : null, OnTransitionOutFinished, false);
            WhileHiding();
        }

        public void Show(IScreenProperties props = null)
        {
            if (props != null && props is TProps typedProps)
            {
                SetProperties(typedProps);
            }
            else
            {
                Debug.LogWarning($"[UIScreenController] Show called with invalid properties for screen {ScreenID}");
                return;
            }

            HierachyFixOnShow();
            OnPropertiesSet();

            if (!gameObject.activeSelf)
            {
                DoAnimation(m_AnimIn, OnTransitionInFinished, true);
            }
            else
            {
                InTransitionFinished?.Invoke(this);
            }
        }

        void DoAnimation(AniComponent caller, Action callWhenFinished, bool isVisible)
        {
            if (caller == null)
            {
                gameObject.SetActive(isVisible);
                callWhenFinished?.Invoke();
            }
            else
            {
                if (isVisible && !gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }

                caller.Animate(transform, callWhenFinished);
            }
        }

        void OnTransitionInFinished()
        {
            IsVisible = true;
            InTransitionFinished?.Invoke(this);
        }

        void OnTransitionOutFinished()
        {
            IsVisible = false;
            gameObject.SetActive(false);
            OutTransitionFinished?.Invoke(this);
        }
    }
}