/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinaryEgo.ToolBox.Attributes;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BinaryEgo.ToolBox.Editor
{
    public class BoundInfo
    {
        public Component component;
        public object boundObject;
        public string boundName;
        public string fieldName;
        public bool auto;

        public BoundInfo(Component p_component, object p_boundObject, string p_boundName, string p_fieldName, bool p_auto)
        {
            component = p_component;
            boundObject = p_boundObject;
            boundName = p_boundName;
            fieldName = p_fieldName;
            auto = p_auto;
        }
    }
    
    [InitializeOnLoad]
    public class BindableUIManager
    {
        private static List<object> _missingBindings;
        private static List<BoundInfo> _bound;
        private static List<object> _missingCallbacks;

        static BindableUIManager()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            
            ValidateBindings();
        }

        static void OnHierarchyChanged()
        {
            if (!EditorApplication.isPlaying)
                ValidateBindings();
        }
        
        static public void ValidateBindings() {
            //Debug.Log("ValidateBindings");
            
            _missingBindings = new List<object>();
            _bound = new List<BoundInfo>();
            _missingCallbacks = new List<object>();

            var stage = PrefabStageUtility.GetCurrentPrefabStage();

            Scene scene = stage == null ? EditorSceneManager.GetActiveScene() : stage.scene;

            GameObject[] allGameObjects = scene.GetRootGameObjects();
            foreach (var gameObject in allGameObjects)
            {
                ValidateGameObject(gameObject);
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        static void ValidateGameObject(GameObject p_gameObject)
        {
            Component[] components = p_gameObject.GetComponents<Component>();
            foreach (var component in components)
            {
                // Yes this can actually happen if you have scripts missing in your project so we need to avoid it
                if (component == null)
                    continue;
                
                FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

                foreach (var field in fields)
                {
                    BindUIElement bindAttribute = field.GetCustomAttribute<BindUIElement>();

                    if (bindAttribute != null)
                    {
                        var lookupName = bindAttribute.customName != "" ? bindAttribute.customName : field.Name;
                        
                        var boundValue = field.GetValue(component);
                        bool nullCheck = false;
                        nullCheck = boundValue == null;
                        nullCheck = nullCheck || (field.FieldType.IsAssignableFrom(typeof(GameObject)) &&
                                                  ((GameObject) boundValue) == null);
                        
                        if (!bindAttribute.overwrite && !nullCheck)
                        {
                            if (boundValue is GameObject)
                            {
                                if (((GameObject) boundValue) != null)
                                {
                                    AddBound(new BoundInfo(component, boundValue, "", field.Name, ((GameObject) boundValue).name == lookupName));
                                    
                                    if (bindAttribute is BindUIButton)
                                    {
                                        AddButtonListeners(((GameObject)boundValue).transform, component, lookupName);
                                    }
                                }
                            } else if (boundValue is Component)
                            {
                                if (((Component) boundValue) != null)
                                {
                                    AddBound(new BoundInfo(component, ((Component) boundValue).gameObject, "",
                                        field.Name, ((Component) boundValue).gameObject.name == lookupName));
                                    
                                    if (bindAttribute is BindUIButton)
                                    {
                                        AddButtonListeners(((Component)boundValue).transform, component, lookupName);
                                    }
                                }
                            }

                            continue;
                        }
                        
                        var boundObject = component.transform.DeepFind(lookupName);
                        if (boundObject == null)
                        {
                            if (!_missingBindings.Contains(p_gameObject))
                                _missingBindings.Add(p_gameObject);
                        }
                        else
                        {
                            AddBound(new BoundInfo(component, boundObject.gameObject, lookupName, field.Name, true));

                            if (bindAttribute is BindUIButton)
                            {
                                AddButtonListeners(boundObject, component, lookupName);
                            }
                            
                            if (field.FieldType.IsAssignableFrom(boundObject.GetType()))
                            {
                                field.SetValue(component, boundObject);
                            }
                            else
                            {
                                if (field.FieldType.IsAssignableFrom(typeof(GameObject)))
                                {
                                    field.SetValue(component, boundObject.gameObject);
                                }
                                else
                                {
                                    field.SetValue(component, boundObject.GetComponent(field.FieldType));
                                }
                            }
                        }
                    }
                }
            }

            foreach (Transform child in p_gameObject.transform)
            {
                ValidateGameObject(child.gameObject);
            }
        }

        static void AddButtonListeners(Transform p_boundObject, Component p_component, string p_lookupName)
        {
            var button = p_boundObject.GetComponent<Button>();
            if (button != null)// && button.autoClickCallback)
            {
                MethodInfo method = UnityEvent.GetValidMethodInfo(p_component, "On" + p_lookupName + "Click", new Type[0]);
                if (method != null)
                {
                    UnityAction methodDelegate = Delegate.CreateDelegate(typeof(UnityAction), p_component, method) as UnityAction;
                    if (button.onClick.GetPersistentEventCount() == 0 ||
                        button.onClick.GetPersistentMethodName(0) != method.Name)
                    {
                        while (button.onClick.GetPersistentEventCount() > 0)
                            UnityEventTools.RemovePersistentListener(button.onClick, 0);
                        
                        UnityEventTools.AddPersistentListener(button.onClick, methodDelegate);
                        EditorUtility.SetDirty(button);
                    }
                }
                else
                {
                    if (!_missingCallbacks.Contains(p_boundObject.gameObject))
                        _missingCallbacks.Add(p_boundObject.gameObject);
                    //_valid = false;
                    Debug.LogWarning("Missing click callback for BindUIButton "+"On" + p_lookupName + "Click");
                }
            }
            else
            {
                if (!_missingCallbacks.Contains(p_boundObject.gameObject))
                    _missingCallbacks.Add(p_boundObject.gameObject);
                //_valid = false;
                Debug.LogWarning("Missing Button component for BindUIButton "+"On" + p_lookupName + "Click on "+p_component);
            }
        }

        static void AddBound(BoundInfo p_boundInfo)
        {
            // TODO multiple bindings can target same object?
            if (!_bound.Exists(bi => bi.boundObject == p_boundInfo.boundObject))
            {
                _bound.Add(p_boundInfo);
            }
        }

        static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (EditorApplication.isPlaying) 
                return;

            Color fontColor = Color.blue;
            Color backgroundColor = new Color(.76f, .76f, .76f);

            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj != null)
            {
                if (_bound.Any(bi => bi.component.gameObject == obj))
                {
                    Rect buttonRect = new Rect(selectionRect.position + new Vector2(selectionRect.size.x - 100, 0),
                        new Vector2(60,16));
                    GUI.Button(buttonRect, "Show");
                }
                
                if (_missingBindings.Contains(obj))
                {
                    fontColor = Color.red;
                    //backgroundColor = new Color(0.24f, 0.48f, 0.90f);

                    Rect offsetRect = new Rect(selectionRect.position + new Vector2(selectionRect.size.x - 100, 2),
                        selectionRect.size);
                    //EditorGUI.DrawRect(selectionRect, backgroundColor);
                    EditorGUI.LabelField(offsetRect, "Missing", new GUIStyle()
                        {
                            normal = new GUIStyleState() {textColor = fontColor},
                            fontStyle = FontStyle.Bold,
                            fontSize = 10
                        }
                    );
                }

                if (_missingCallbacks.Contains(obj))
                {
                    fontColor = Color.red;
                    //backgroundColor = new Color(0.24f, 0.48f, 0.90f);

                    Rect offsetRect = new Rect(selectionRect.position + new Vector2(selectionRect.size.x - 100, 2),
                        selectionRect.size);
                    //EditorGUI.DrawRect(selectionRect, backgroundColor);
                    EditorGUI.LabelField(offsetRect, "Failed", new GUIStyle()
                        {
                            normal = new GUIStyleState() {textColor = fontColor},
                            fontStyle = FontStyle.Bold,
                            fontSize = 10
                        }
                    );
                }

                BoundInfo boundInfo = _bound.Find(bi => bi.boundObject == obj);

                if (boundInfo != null && !_missingCallbacks.Contains(obj))
                {
                    fontColor = boundInfo.auto ? Color.green : Color.cyan;
                    //backgroundColor = new Color(0.24f, 0.48f, 0.90f);

                    Rect offsetRect = new Rect(selectionRect.position + new Vector2(selectionRect.size.x - 100, 2),
                        selectionRect.size);
                    //EditorGUI.DrawRect(selectionRect, backgroundColor);

                    string tooltip = boundInfo.component != null
                        ? "Bound to " + boundInfo.component.gameObject.name + " as field " + boundInfo.fieldName
                        : "Bound info missing.";
                        
                    EditorGUI.LabelField(offsetRect, new GUIContent("Bound", tooltip), new GUIStyle()
                        {
                            normal = new GUIStyleState() {textColor = fontColor},
                            fontStyle = FontStyle.Bold,
                            fontSize = 10
                        }
                    );
                }
            }
        }
    }
}