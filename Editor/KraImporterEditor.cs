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
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var currentProperty = serializedObject.GetIterator();

            do
            {
                var propField = CreatePropertyField(currentProperty);
                if (currentProperty.propertyPath == "m_Script")
                    propField.SetEnabled(false);
                root.Add(propField);
            } while (currentProperty.NextVisible(currentProperty.name == "Base"));
            
            var type = Type.GetType("UnityEditor.U2D.Sprites.SpriteEditorWindow,Unity.2D.Sprite.Editor");
            var showSpriteEditor = new Button() { text = "Show Sprite Editor" };
            showSpriteEditor.clicked += () => { EditorWindow.GetWindow(type).Show(); };
            root.Add(showSpriteEditor);

            var imgui = new IMGUIContainer();
            imgui.onGUIHandler += IMGUI;
            root.Add(imgui);

            return root;
        }

        private void IMGUI()
        {
            ApplyRevertGUI();
        }

        private VisualElement CreatePropertyField(SerializedProperty property)
        {
            return new PropertyField { label = property.displayName, bindingPath = property.propertyPath };
        }

    }
}