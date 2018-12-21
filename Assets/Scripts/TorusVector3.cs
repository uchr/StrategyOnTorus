using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TorusVector3 {
    float sR; // small radious
    float bR; // big radious
    float xA; // x angle
    float yA; // y angle

    static Vector3 ToCartesian(TorusVector3 tv) {
        Vector3 v;
        v.x = (tv.bR + tv.sR * Mathf.Cos(tv.xA)) * Mathf.Cos(tv.yA);
        v.y = tv.sR * Mathf.Sin(tv.xA);
        v.z = (tv.bR + tv.sR * Mathf.Cos(tv.xA)) * Mathf.Sin(tv.yA);
        return v;
    }
}
