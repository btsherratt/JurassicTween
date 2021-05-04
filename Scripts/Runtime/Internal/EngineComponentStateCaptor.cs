//
// EngineComponentStateCaptor.cs
// JurassicTween
//
// Created by Benjamin Sherratt on 04/05/2021.
// Copyright © 2021 SKFX Ltd. All rights reserved.
//

using System.Collections.Generic;
using UnityEngine;

namespace JurassicTween.Internal {
    public class TransformComponentStateCaptor : IComponentStateCaptor {
        static string[] animationKeys = {
            "localPosition.x",
            "localPosition.y",
            "localPosition.z",
            "localRotation.x",
            "localRotation.y",
            "localRotation.z",
            "localRotation.w",
            "localScale.x",
            "localScale.y",
            "localScale.z",
        };

        public ComponentState GetComponentState(Component component) {
            Transform transform = (Transform)component;

            float[] stateValues = new float[10];
            stateValues[0] = transform.localPosition.x;
            stateValues[1] = transform.localPosition.y;
            stateValues[2] = transform.localPosition.z;
            stateValues[3] = transform.localRotation.x;
            stateValues[4] = transform.localRotation.y;
            stateValues[5] = transform.localRotation.z;
            stateValues[6] = transform.localRotation.w;
            stateValues[7] = transform.localScale.x;
            stateValues[8] = transform.localScale.y;
            stateValues[9] = transform.localScale.z;

            ComponentState state = new ComponentState(stateValues);
            return state;
        }

        public ComponentStateDelta GenerateComponentStateDelta(ComponentState startState, ComponentState endState) {
            Dictionary<string, ComponentStateDelta.Range> rangeByAnimationKey = new Dictionary<string, ComponentStateDelta.Range>();

            for (uint i = 0; i < animationKeys.Length; ++i) {
                if (startState.values[i] != endState.values[i]) {
                    rangeByAnimationKey[animationKeys[i]] = new ComponentStateDelta.Range(startState.values[i], endState.values[i]);
                }
            }

            ComponentStateDelta delta = new ComponentStateDelta(rangeByAnimationKey);
            return delta;
        }
    }
}
