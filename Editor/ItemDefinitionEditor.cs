using System.Collections.Generic;
using System.Linq;
using Dave6.CharacterKit.Item;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Stat;
using UnityEditor;
using UnityEngine;

namespace Dave6.CharacterKitEditor
{
    [CustomEditor(typeof(ItemDefinition))]
    public class ItemDefinitionEditor : Editor
    {
        ItemDefinition definition;

        SerializedProperty icon;
        SerializedProperty displayName;
        SerializedProperty worldPrefab;

        SerializedProperty category;
        SerializedProperty allowedSlots;
        SerializedProperty activePrefab;

        EItemCategory prevCategory;
        bool slotsAutoCleaned;
        

        SerializedProperty affectMode;
        SerializedProperty statValueOptions;
        SerializedProperty valueOperationOptions;



        void OnEnable()
        {
            definition = (ItemDefinition)target;

            icon = serializedObject.FindProperty("icon");
            displayName = serializedObject.FindProperty("displayName");
            worldPrefab = serializedObject.FindProperty("worldPrefab");
            category = serializedObject.FindProperty("category");
            allowedSlots = serializedObject.FindProperty("allowedSlots");
            activePrefab = serializedObject.FindProperty("activePrefab");

            prevCategory = (EItemCategory)category.enumValueIndex;


            affectMode = serializedObject.FindProperty("affectMode");
            statValueOptions = serializedObject.FindProperty("statValueOptions");
            valueOperationOptions = serializedObject.FindProperty("valueOperationOptions");

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(icon);
            EditorGUILayout.PropertyField(displayName);
            EditorGUILayout.PropertyField(worldPrefab);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8);

            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(category);
            if (EditorGUI.EndChangeCheck())
            {
                AutoCleanAllowedSlots();
            }
            EditorGUILayout.PropertyField(allowedSlots, true);
            EditorGUILayout.PropertyField(activePrefab);
            EditorGUILayout.EndVertical();

            if (slotsAutoCleaned)
            {
                EditorGUILayout.HelpBox("허용 슬롯이 아이템 카테고리에 맞지 않는 항목이 있어 자동으로 정리되었습니다.", MessageType.Info);
            }
            
            EditorGUILayout.Space(16);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(affectMode);
            DrawAffectOptions();
            EditorGUILayout.EndVertical();


            serializedObject.ApplyModifiedProperties();
        }

        void DrawAffectOptions()
        {
            var mode = (EStatAffectMode)affectMode.enumValueIndex;
            switch (mode)
            {
                case EStatAffectMode.StatValueType:
                    EditorGUILayout.PropertyField(statValueOptions, true);
                    break;
                case EStatAffectMode.ValueOperationType:
                    EditorGUILayout.PropertyField(valueOperationOptions, true);
                    break;
            }
        }

        void AutoCleanAllowedSlots()
        {
            slotsAutoCleaned = false;

            var newCategory = (EItemCategory)category.enumValueIndex;
            if (newCategory == prevCategory)
                return;

            prevCategory = newCategory;

            if (!ItemSlotRule.map.TryGetValue(newCategory, out var validSlots))
                return;

            Undo.RecordObject(target, "Auto Clean Allowed Slots");

            for (int i = allowedSlots.arraySize - 1; i >= 0; i--)
            {
                var element = allowedSlots.GetArrayElementAtIndex(i);
                var slot = (EEquipSlotType)element.enumValueIndex;

                if (!validSlots.Contains(slot))
                {
                    allowedSlots.DeleteArrayElementAtIndex(i);
                    slotsAutoCleaned = true;
                }
            }
        }
    }

    /// <summary>
    /// 카테고리에 맞게 선택할 수 있는 슬롯을 제한하는 룰
    /// </summary>
    public static class ItemSlotRule
    {
        public static readonly Dictionary<EItemCategory, EEquipSlotType[]> map = new()
        {
            { EItemCategory.Weapon, new [] { EEquipSlotType.PrimaryWeapon, EEquipSlotType.SecondaryWeapon, EEquipSlotType.MeleeWeapon } },
            { EItemCategory.Armor, new [] { EEquipSlotType.Head, EEquipSlotType.Chest, EEquipSlotType.Leg, EEquipSlotType.Charm } },
            { EItemCategory.Consumable, new [] { EEquipSlotType.ConsumableA, EEquipSlotType.ConsumableB } },
        };
    }

    [CustomPropertyDrawer(typeof(EEquipSlotType))]
    public class EquipSlotTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var itemDef = property.serializedObject.targetObject as ItemDefinition;
            if (itemDef == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (!ItemSlotRule.map.TryGetValue(itemDef.category, out var validSlots))
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // 현재 값
            var current = (EEquipSlotType)property.enumValueIndex;

            // 제한된 슬롯 중 인덱스
            int currentIndex = Mathf.Max(0, System.Array.IndexOf(validSlots, current));

            // Popup 표시용 이름
            var displayNames = validSlots.Select(s => ObjectNames.NicifyVariableName(s.ToString())).ToArray();

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, displayNames);
            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = (int)validSlots[newIndex];
            }
        }
    }

    [CustomPropertyDrawer(typeof(StatValueOption))]
    public class ItemOptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tag = property.FindPropertyRelative("tag");
            var valueType = property.FindPropertyRelative("valueType");
            var magnitude = property.FindPropertyRelative("magnitude");

            string summary = GetSummary(tag, valueType, magnitude);

            EditorGUI.PropertyField(position, property, new GUIContent(summary), true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        string GetSummary(SerializedProperty tag, SerializedProperty valueType, SerializedProperty magnitude)
        {
            var statTag = tag.objectReferenceValue as StatTag;
            var name = statTag != null ? statTag.tagName : "None";
            string valueTypeName = ((EStatValueType)valueType.enumValueIndex).ToString();

            string sign = "";

            switch (valueType.enumValueIndex)
            {
                case (int)EStatValueType.Percent:
                    sign = "%";
                break;
                case (int)EStatValueType.finalMultiplier:
                    sign = "x";
                break;
            }

            float magValue = magnitude.floatValue;

            return $"{name} | {valueTypeName} | {magValue}{sign}";
        }
    }

    [CustomPropertyDrawer(typeof(ValueOperationOption))]
    public class ValueOperationOptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tag = property.FindPropertyRelative("tag");
            var operationType = property.FindPropertyRelative("operationType");
            var magnitude = property.FindPropertyRelative("magnitude");

            string summary = GetSummary(tag, operationType, magnitude);

            EditorGUI.PropertyField(position, property, new GUIContent(summary), true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        string GetSummary(SerializedProperty tag, SerializedProperty operationType, SerializedProperty magnitude)
        {
            var statTag = tag.objectReferenceValue as StatTag;
            var name = statTag != null ? statTag.tagName : "None";
            string valueTypeName = ((EValueOperationType)operationType.enumValueIndex).ToString();

            string sign = "";

            switch (operationType.enumValueIndex)
            {
                case (int)EValueOperationType.Current:
                    sign = "";
                break;
                case (int)EValueOperationType.CurrentPercent:
                case (int)EValueOperationType.MaxPercent:
                    sign = "%";
                break;
            }

            float magValue = magnitude.floatValue;

            return $"{name} | {valueTypeName} | {magValue}{sign}";
        }
    }
}
