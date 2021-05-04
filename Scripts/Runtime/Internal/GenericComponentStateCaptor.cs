//
// GenericComponentStateCaptor.cs
// JurassicTween
//
// Created by Benjamin Sherratt on 04/05/2021.
// Copyright © 2021 SKFX Ltd. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace JurassicTween.Internal {
    public class GenericComponentStateCaptor : IComponentStateCaptor {
        struct FieldCaptureInfo {
            public string animationKeyPath;
            public MemberInfo[] memberInfoHierarchy;

            public FieldCaptureInfo(string animationKeyPath, MemberInfo[] memberInfoHierarchy) {
                this.animationKeyPath = animationKeyPath;
                this.memberInfoHierarchy = memberInfoHierarchy;
            }
        }

        static FieldCaptureInfo[] QueryCaptureFields(Type type) {
            List<FieldCaptureInfo> fieldCaptureInfo = new List<FieldCaptureInfo>();
            List<string> keyComponents = new List<string>();
            List<MemberInfo> memberInfoHierarchy = new List<MemberInfo>();
            QueryCaptureFields(fieldCaptureInfo, type, keyComponents, memberInfoHierarchy);
            return fieldCaptureInfo.ToArray();
        }

        static void QueryCaptureFields(List<FieldCaptureInfo> fieldCaptureInfo, Type type, List<string> keyComponents, List<MemberInfo> memberInfoHierarchy) {
            FieldInfo[] allFieldInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in allFieldInfo) {
                if (fieldInfo.IsPublic || fieldInfo.GetCustomAttribute<SerializeField>() != null) {
                    keyComponents.Add(fieldInfo.Name);
                    memberInfoHierarchy.Add(fieldInfo);

                    Type fieldType = fieldInfo.FieldType;
                    if (fieldType.IsAssignableFrom(typeof(float))) {
                        string animationKey = string.Join(".", keyComponents);
                        FieldCaptureInfo info = new FieldCaptureInfo(animationKey, memberInfoHierarchy.ToArray());
                        fieldCaptureInfo.Add(info);
                    } else {
                        QueryCaptureFields(fieldCaptureInfo, fieldType, keyComponents, memberInfoHierarchy);
                    }

                    memberInfoHierarchy.RemoveAt(memberInfoHierarchy.Count - 1);
                    keyComponents.RemoveAt(keyComponents.Count - 1);
                }
            }
        }

        Type componentType;
        FieldCaptureInfo[] fieldCaptureInformation;

        public GenericComponentStateCaptor(Type componentType) {
            this.componentType = componentType;
            fieldCaptureInformation = QueryCaptureFields(componentType);
        }

        public ComponentState GetComponentState(Component component) {
            float[] stateValues = new float[fieldCaptureInformation.Length];

            for (uint i = 0; i < fieldCaptureInformation.Length; ++i) {
                ref FieldCaptureInfo fieldCaptureInfo = ref fieldCaptureInformation[i];
                object instanceScope = component;
                foreach (MemberInfo memberInfo in fieldCaptureInfo.memberInfoHierarchy) {
                    if ((memberInfo.MemberType & MemberTypes.Field) > 0) {
                        instanceScope = ((FieldInfo)memberInfo).GetValue(instanceScope);
                    } else if ((memberInfo.MemberType & MemberTypes.Property) > 0) {
                        instanceScope = ((PropertyInfo)memberInfo).GetValue(instanceScope);
                    } else {
                        throw new Exception("We shouldn't be here...");
                    }
                }

                float value = (float)instanceScope;
                stateValues[i] = value;
            }

            ComponentState state = new ComponentState(stateValues);
            return state;
        }

        public ComponentStateDelta GenerateComponentStateDelta(ComponentState startState, ComponentState endState) {
            Dictionary<string, ComponentStateDelta.Range> rangeByAnimationKey = new Dictionary<string, ComponentStateDelta.Range>();
            for (uint i = 0; i < fieldCaptureInformation.Length; ++i) {
                float startValue = startState.values[i];
                float endValue = endState.values[i];
                if (startValue != endValue) {
                    ref FieldCaptureInfo fieldCaptureInfo = ref fieldCaptureInformation[i];
                    rangeByAnimationKey[fieldCaptureInfo.animationKeyPath] = new ComponentStateDelta.Range(startValue, endValue);
                }
            }

            ComponentStateDelta delta = new ComponentStateDelta(rangeByAnimationKey);
            return delta;
        }
    }
}
