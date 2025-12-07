using System.Collections;
using System.Collections.Generic;
using UIFramework.Core;
using UnityEngine;

namespace UIFramework
{
    [CreateAssetMenu(fileName = "UISetting", menuName = "UI/UISetting")]
    public class UISettings : ScriptableObject
    {
        [SerializeField] UIFrame m_TemplateUIPrefab = null;
        [SerializeField] List<GameObject> m_ScreensToRegister = null;
        [SerializeField] bool m_DeactivateScreensOnLoad = true;

        public UIFrame CreateUIInstance(bool instanceAndRegisterScreens = true)
        {
            UIFrame uiInstance = Instantiate(m_TemplateUIPrefab);
            if (instanceAndRegisterScreens)
            {
                foreach (var screen in m_ScreensToRegister)
                {
                    var screenInstance = Instantiate(screen);
                    var screenController = screenInstance.GetComponent<IScreenController>();

                    if (screenController != null)
                    {
                        uiInstance.RegisterScreen(screen.name, screenController, screenInstance.transform);
                        if (m_DeactivateScreensOnLoad && screenInstance.activeSelf)
                        {
                            screenInstance.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"The screen prefab {screen.name} does not have a component that implements IScreenController.");
                    }
                }
            }
            return uiInstance;
        }

        void OnValidate()
        {
            List<GameObject> objectsToRemove = new List<GameObject>();
            for (int i = 0; i < m_ScreensToRegister.Count; i++)
            {
                var screenCtl = m_ScreensToRegister[i].GetComponent<IScreenController>();
                if (screenCtl == null)
                {
                    objectsToRemove.Add(m_ScreensToRegister[i]);
                }
            }

            if (objectsToRemove.Count > 0)
            {
                foreach (var obj in objectsToRemove)
                {
                    m_ScreensToRegister.Remove(obj);
                    Debug.LogWarning($"Removed {obj.name} from ScreensToRegister because it does not implement IScreenController.");
                }
            }
        }

    }
}