# JurassicTween

> Your scientists were so preoccupied with whether or not they could (tween like that) they didn’t stop to think if they should.

🚨 This is a preview only. I advise against shipping this. 🚨

## What?

JurassicTween is a Unity3D tweening library designed for:

* Tweening values on `Component` objects...
* ...in a very simple way...
* ...favoring ease of prototyping over performance (for now).

## How?

Add JurassicTween to a project. Making a tween is super easy, here's how you would make an object randomly move, rotate and scale onscreen:

```
using JurassicTween;
using UnityEngine;

public class MoveMe : MonoBehaviour {
    void Update() {
        if (gameObject.IsTweening() == false) {
            using (transform.Tween()) {
                transform.position = Random.insideUnitSphere * 10.0f;
                transform.rotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
                transform.localScale = Vector3.one * Random.Range(0.1f, 2.0f);
            }
            // using (anyOtherComponent.Tween()) {
                // These chain together, and your own MonoBehavior components should also work...
            // }
        }
    }
}
```

Basically, the tween breaks down as follows:

* Mark a game component as requiring a tween using `Component.Tween()`
* Set the fields on the component that you want to target
* The tween will then be set up to be from the current values to the target values you set.

The tweens themselves are performed on a component, and you can check if a game object currently has any active tweens.

**VERY IMPORTANT:** Not everything will work correctly, especially Unity's native components. (Transform is an exception, we've fixed that.)

## Roadmap

This is a preview release only. There are bugs. Lots of things won't work.

Currently the following features are supported:

* Some things tween
* You can give a duration of tween
* The curve of the tweens can be set, and you can use an AnimationCurve to design your own.

If this was useful to people and there was a sustainable way to develop it, we could add:

* More API features
* Better animation system and/or better integration into the Unity systems
* Performance!

If you like this then please let me know on Twitter, find me at http://twitter.com/btsherratt/.

# Licence

Currently this is licenced for evaluation use in your non-commercial projects only, and all rights are currently reserved and retained by my company SKFX Ltd.. If there is interest in taking this forward then this will be changed, but I would like to decide slowly about what makes most sense for licensing here.