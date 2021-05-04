//
// Context.cs
// JurassicTween
//
// Created by Benjamin Sherratt on 04/05/2021.
// Copyright © 2021 SKFX Ltd. All rights reserved.
//

using System;
using UnityEngine;

namespace JurassicTween.Internal {
    class Context : IDisposable {
        private Component component;
        private IComponentStateCaptor componentStateCaptor;
        private ComponentState startState;
        private float time;
        private TweenParameters tweenParameters;

        public Context(Component component, float time, TweenParameters tweenParameters) {
            this.component = component;
            componentStateCaptor = component.GetStateCaptor();
            startState = componentStateCaptor.GetComponentState(component);

            this.time = time;
            this.tweenParameters = tweenParameters;
        }

        public void Dispose() {
            ComponentState endState = componentStateCaptor.GetComponentState(component);

            ComponentStateDelta delta = componentStateCaptor.GenerateComponentStateDelta(startState, endState);

            Animation animation = component.gameObject.GetComponent<Animation>();
            if (animation == null) {
                animation = component.gameObject.AddComponent<Animation>();
            }

            string currentClipName = "jurassicTween";
            AnimationClip animationClip = animation.GetClip(currentClipName);
            if (animationClip == null) {
                animationClip = new AnimationClip();
                animationClip.legacy = true;
                animationClip.name = currentClipName;
                animation.AddClip(animationClip, animationClip.name);
            }

            foreach (string key in delta.rangeByAnimationKey.Keys) {
                ComponentStateDelta.Range range = delta.rangeByAnimationKey[key];

                AnimationCurve curve;
                switch (tweenParameters.interpolation) {
                case TweenParameters.Interpolation.Linear:
                    curve = AnimationCurve.Linear(0, range.start, time, range.end);
                    break;

                case TweenParameters.Interpolation.EaseInOut:
                    curve = AnimationCurve.EaseInOut(0, range.start, time, range.end);
                    break;

                case TweenParameters.Interpolation.CustomCurve:
                    curve = new AnimationCurve();
                    foreach (Keyframe referenceKeyframe in tweenParameters.customCurve.keys) {
                        Keyframe keyframe = new Keyframe(
                            Mathf.Lerp(0.0f, time, referenceKeyframe.time),
                            Mathf.Lerp(range.start, range.end, referenceKeyframe.value),
                            referenceKeyframe.inTangent,
                            referenceKeyframe.outTangent,
                            referenceKeyframe.inWeight,
                            referenceKeyframe.outWeight);
                        curve.AddKey(keyframe);
                    }
                    break;

                default:
                    throw new Exception("We shouldn't be here");
                }

                animationClip.SetCurve("", component.GetType(), key, curve);
            }

            animation.Play(animationClip.name);
        }
    }
}
