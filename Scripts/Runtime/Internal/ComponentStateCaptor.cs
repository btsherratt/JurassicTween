//
// ComponentStateCaptor.cs
// JurassicTween
//
// Created by Benjamin Sherratt on 04/05/2021.
// Copyright © 2021 SKFX Ltd. All rights reserved.
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace JurassicTween.Internal {
    public struct ComponentState {
        public float[] values;

        public ComponentState(float[] values) {
            this.values = values;
        }
    }
    public struct ComponentStateDelta {
        public struct Range {
            public float start;
            public float end;

            public Range(float start, float end) {
                this.start = start;
                this.end = end;
            }
        }

        public Dictionary<string, Range> rangeByAnimationKey;

        public ComponentStateDelta(Dictionary<string, Range> rangeByAnimationKey) {
            this.rangeByAnimationKey = rangeByAnimationKey;
        }
    }

    public interface IComponentStateGetter {
        ComponentState GetComponentState(Component component);
    }

    public interface IComponentStateDeltaGenerator {
        ComponentStateDelta GenerateComponentStateDelta(ComponentState startState, ComponentState endState);
    }

    public interface IComponentStateCaptor : IComponentStateGetter, IComponentStateDeltaGenerator {
    }

    public static class ComponentStateCaptor {
        static Dictionary<Type, IComponentStateCaptor> stateCaptorByType;

        public static IComponentStateCaptor GetStateCaptor(this Component component) {
            Type componentType = component.GetType();

            if (stateCaptorByType == null) {
                stateCaptorByType = new Dictionary<Type, IComponentStateCaptor>();
                CreateUnityInternalCaptors();
            }

            IComponentStateCaptor stateCaptor;
            if (stateCaptorByType.ContainsKey(componentType)) {
                stateCaptor = stateCaptorByType[componentType];
            } else {
                stateCaptor = new GenericComponentStateCaptor(componentType);
                stateCaptorByType[componentType] = stateCaptor;
            }

            return stateCaptor;
        }

        static void CreateUnityInternalCaptors() {
            // Add the custom captors for Unity internals...
            stateCaptorByType[typeof(Transform)] = new TransformComponentStateCaptor();
        }
    }
}
