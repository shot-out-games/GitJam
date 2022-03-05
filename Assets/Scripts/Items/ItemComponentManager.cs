using System.Collections;
using System.Collections.Generic;
using SandBox.Player;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public struct PowerItemComponent : IComponentData
{
    public Entity pickedUpActor;
    public bool active;
    public bool enabled;
    public Entity particleSystemEntity;
    public Entity addPickupEntityToInventory;

}



public struct ImmediateUseComponent : IComponentData
{
    public float value;
}

public struct ParticleSystemComponent : IComponentData
{
    public float value;
    public bool followActor;
    public Entity pickedUpActor;
}

public struct AudioSourceComponent : IComponentData
{
    public bool active;
}


public struct Speed : IComponentData
{

    public Entity psAttached;
    public Entity pickedUpActor;
    public Entity itemEntity;
    public bool triggered;
    public bool enabled;
    public bool startTimer;
    public float timer;
    public float timeOn;
    public float originalSpeed;
    public float multiplier;
}


public struct ControlPower : IComponentData
{
    public Entity psAttached;
    public Entity pickedUpActor;
    public Entity itemEntity;
    public bool enabled;
    public float controlMultiplier;
}


