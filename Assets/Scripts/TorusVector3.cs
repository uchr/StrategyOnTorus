using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TorusVector3 {
    public float sR; // small radious
    public float bR; // big radious
    public float xA; // x angle
    public float yA; // y angle

    public TorusVector3(float sR, float bR, float xA, float yA) {
        this.sR = sR;
        this.bR = bR;
        this.xA = xA;
        this.yA = yA;
    }

    public Vector3 ToCartesian() {
        return ToCartesian(this);
    }

    public static Vector3 ToCartesian(TorusVector3 tv) {
        Vector3 v;
        v.x = (tv.bR + tv.sR * Mathf.Cos(tv.xA)) * Mathf.Cos(tv.yA);
        v.y = tv.sR * Mathf.Sin(tv.xA);
        v.z = (tv.bR + tv.sR * Mathf.Cos(tv.xA)) * Mathf.Sin(tv.yA);
        return v;
    }
}
