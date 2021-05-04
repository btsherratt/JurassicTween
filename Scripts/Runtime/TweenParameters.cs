//
// TweenParameters.cs
// JurassicTween
//
// Created by Benjamin Sherratt on 04/05/2021.
// Copyright © 2021 SKFX Ltd. All rights reserved.
//

using UnityEngine;

namespace JurassicTween {
    public struct TweenParameters {
        public static readonly TweenParameters linear = new TweenParameters(Interpolation.Linear);
        public static readonly TweenParameters easeInOut = new TweenParameters(Interpolation.EaseInOut);

        public enum Interpolation {
            Linear,
            EaseInOut,
            CustomCurve,
        }

        public Interpolation interpolation;
        public AnimationCurve customCurve;

        public TweenParameters(AnimationCurve customCurve) {
            interpolation = Interpolation.CustomCurve;
            this.customCurve = customCurve;
        }

        TweenParameters(Interpolation interpolation) {
            this.interpolation = interpolation;
            this.customCurve = null;
        }
    }
}
