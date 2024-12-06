using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.AssetImporters;

namespace HNL
{
    [CustomEditor(typeof(KraImporter))]
    [CanEditMultipleObjects]
    public class KraImporterEditor : ScriptedImporterEditor
    {
        private event Action<KraImporter.TextureType> textureTypeChanged;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var currentProperty = serializedObject.GetIterator();

            do
            {
                var propField = CreatePropertyField(currentProperty);
                
                if (currentProperty.propertyPath == "m_Script")
                {
                    // 禁用脚本属性栏
                    // Disable script field
                    propField.SetEnabled(false);
                }
                else if (currentProperty.propertyPath == "textureType")
                {
                    // 触发 textureTypeChanged
                    // Invoke textureTypeChanged
                    propField.RegisterValueChangeCallback(evt =>
                    {
                        var prop = evt.changedProperty;
                        textureTypeChanged?.Invoke((KraImporter.TextureType)prop.enumValueIndex);
                    });
                }
                else if (currentProperty.propertyPath == "spriteSettings")
                {
                    // 根据 textureType 决定是否显示 spriteSettings
                    // Show spriteSettings if textureType is Sprite
                    var textureTypeProperty = serializedObject.FindProperty("textureType");
                    var textureType = (KraImporter.TextureType)textureTypeProperty.enumValueIndex;
                    
                    Action<KraImporter.TextureType> action = (type) =>
                    {
                        if (type == KraImporter.TextureType.Sprite)
                            propField.style.display = DisplayStyle.Flex;
                        else
                            propField.style.display = DisplayStyle.None;
                    };
                    action.Invoke(textureType);
                    textureTypeChanged += action;
                }

                root.Add(propField);
            } while (currentProperty.NextVisible(currentProperty.name == "Base"));
            
            // var type = Type.GetType("UnityEditor.U2D.Sprites.SpriteEditorWindow,Unity.2D.Sprite.Editor");
            // var showSpriteEditor = new Button() { text = "Show Sprite Editor" };
            // showSpriteEditor.clicked += () => { EditorWindow.GetWindow(type).Show(); };
            // root.Add(showSpriteEditor);

            var imgui = new IMGUIContainer();
            imgui.onGUIHandler += IMGUI;
            root.Add(imgui);

            return root;
        }

        private void IMGUI()
        {
            ApplyRevertGUI();
        }

        private PropertyField CreatePropertyField(SerializedProperty property)
        {
            return new PropertyField { label = property.displayName, bindingPath = property.propertyPath };
        }

    }
}