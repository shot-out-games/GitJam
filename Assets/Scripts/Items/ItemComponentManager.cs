
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using System;
using System.Collections.Generic;
using UnityEngine;



public struct UseItem1 : IComponentData
{

}
public struct UseItem2 : IComponentData
{

}


public struct PowerItemComponent : IComponentData
{
    public Entity pickedUpActor;
    public Entity pickupEntity;
    public bool active;
    public bool enabled;
    public Entity particleSystemEntity;
    public Entity addPickupEntityToInventory;
    public bool itemPickedUp;
    public FixedString64 description;
    public bool buttonAssigned;
    public bool useSlot1;
    public bool useSlot2;
    public int index;
    public PickupType pickupType;
    public int count;

}


public struct PowerItemIndexComparer : IComparer<PowerItemComponent> 
{
    

    public int Compare(PowerItemComponent a, PowerItemComponent b)
    {
      
        int a_index = a.index;
        int b_index = b.index;
        if (a_index > b_index)
            return 1;
        else if (a_index < b_index)
            return -1;
        else
            return 0;

    }
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
    public int count;
}


public struct ControlPower : IComponentData
{
    public Entity psAttached;
    public Entity pickedUpActor;
    public Entity itemEntity;
    public bool enabled;
    public float controlMultiplier;
}



[InternalBufferCapacity(24)]
public struct PickupListBuffer : IBufferElementData
{
    public Entity e;
    public Entity _parent;
    public bool active;
    public bool pickedUp;
    public bool special;//for ld
    public bool reset;
    public bool playerPickupAllowed;
    public bool enemyPickupAllowed;
    public int index;
    public int statl;
    public float3 position;
}


public struct PickupComponent : IComponentData
{
    public Entity e;
    public Entity _parent;
    public bool active;
    public bool pickedUp;
    public bool special;//for ld
    public bool reset;
    public bool playerPickupAllowed;
    public bool enemyPickupAllowed;
    public int index;
    public int statl;

}

public struct PickupManagerComponent : IComponentData //used for managed components - read and then call methods from MB
{
    public bool playSound;
    public bool setAnimationLayer;
}


public enum PickupType
{
    None,
    Speed,
    Health,
    Dash,
    Control
}

[System.Serializable]
public class ItemClass
{
    public Transform location;
    public GameObject ItemPrefab;
}



