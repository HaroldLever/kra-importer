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
            } while (currentProperty.NextVisible(true));

            var imgui = new IMGUIContainer();
            imgui.onGUIHandler += IMGUI;
            root.Add(imgui);

            return root;
        }

        // public override void OnInspectorGUI()
        // {
        //     serializedObject.Update();
        //     serializedObject.ApplyModifiedProperties();
        //     base.ApplyRevertGUI();
        // }

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