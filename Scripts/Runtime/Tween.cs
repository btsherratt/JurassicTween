//
// Tween.cs
// JurassicTween
//
// Created by Benjamin Sherratt on 04/05/2021.
// Copyright © 2021 SKFX Ltd. All rights reserved.
//

using JurassicTween.Internal;
using System;
using UnityEngine;

namespace JurassicTween {
    public static class JurassicTween {
        public static IDisposable Tween(this Component component, float time = 1.0f) {
            return component.Tween(time, TweenParameters.linear);
        }

        public static IDisposable Tween(this Component component, float time, TweenParameters tweenParameters) {
            Context context = new Context(component, time, tweenParameters);
            return context;
        }

        public static bool IsTweening(this GameObject gameObject) {
            Animation animation = gameObject.GetComponent<Animation>();
            bool tweening = ((animation != null) && animation.isPlaying);
            return tweening;
        }
    }
}
