using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;

public class SpacePropRandomRotZ : TransformationRule {

    public override void GetTransform(PropSocket socket, DungeonModel model, Matrix4x4 propTransform, System.Random random, out Vector3 outPosition, out Quaternion outRotation, out Vector3 outScale)
    {
        outPosition = Vector3.zero;
        outRotation = Quaternion.Euler(0, random.NextFloat() * 360, 0);
        outScale = Vector3.one;
    }
}
