using System;
using System.Xml.Linq;
using Rewired;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using SandBox.Player;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine.VFX;

public struct PlayerMoveComponent : IComponentData
{
    //public float currentSpeed;
    public float negativeForce;
    public float rotateSpeed;
    public bool snapRotation;
    public float dampTime;
    public bool move2d;
    public float3 startPosition;
    public bool inputDisabled;

}



namespace SandBox.Player
{



    public class PlayerMove : MonoBehaviour, IConvertGameObjectToEntity
    {



        public AudioSource audioSource;
        public AudioClip clip;
        //public ParticleSystem psPrefab;
        //public ParticleSystem psInstance;

        public GameObject psPrefab;
        public Transform psParent;
        public GameObject psInstance;


        public float fallingFramesMax;
        public float negativeForce = -9.81f;
        public float approachStairBoost = 9.81f;
        [SerializeField]
        float rotateSpeed = 9;
        [SerializeField]
        float dampTime = .03f;
        [SerializeField]
        bool snapRotation = true;
        [SerializeField]
        bool move2d = false;

        [HideInInspector]
        public Camera mainCam;

        float startSpeed = 4f;
        //float currentSpeed = 4f;
        private EntityManager _entityManager;
        private Entity _entity;

        //[SerializeField]
        //float negativeForce = -9.81f;
        private Animator animator;
        public int targetFrameRate = -1;
        public bool inputDisabled;
        void Start()
        {
            if (targetFrameRate >= 10)
            {
                Application.targetFrameRate = targetFrameRate;
            }
            var ps = Instantiate(psPrefab, psParent);
            ps.transform.position = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
            psInstance = ps;
            psInstance.GetComponent<VisualEffect>().Play();
            animator = GetComponent<Animator>();
            mainCam = Camera.main;

        }

        //void OnAnimatorMove()
        //{
        //    transform.position += animator.deltaPosition;
        //    transform.forward = animator.deltaRotation * transform.forward;
        //    //transform.rotation = animator.deltaRotation;

        //}

        //private void OnAnimatorMove()
        private void Update()
        {

            if (_entity == Entity.Null) return;
            //changing so we need root animation here only
            if (!ReInput.isReady) return;



        }

        void LateUpdate()
        {
            //if (Terrain.activeTerrain != null && !jumpEnabled)
            //{
            //    Vector3 newpos = transform.position;
            //    newpos.y = Terrain.activeTerrain.SampleHeight(transform.position);
            //    transform.position = newpos;
            //}

        }


        public void FacePlayer(Transform target = null, float rotateSpeed = .01f)
        {
            Vector3 lookDir = target.position - transform.position;
            lookDir.y = 0;
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

        }



        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            _entity = entity;
            _entityManager = dstManager;

            startSpeed = GetComponent<PlayerRatings>() ? GetComponent<PlayerRatings>().Ratings.speed : 4f;
            //if (move2d)
            //{
            //snapRotation = true;
            //dampTime = 0;
            //}


            dstManager.AddComponentData(entity, new PlayerMoveComponent()
            {
                //negativeForce = negativeForce,
                //currentSpeed = startSpeed,
                rotateSpeed = rotateSpeed,
                snapRotation = snapRotation,
                dampTime = dampTime,
                move2d = move2d,
                startPosition = transform.position,
                inputDisabled = inputDisabled


            });


            dstManager.AddComponentData(entity, new ApplyImpulseComponent { Force = 0, Direction = Vector3.zero, Grounded = false, fallingFramesMaximuim = fallingFramesMax, NegativeForce = negativeForce, ApproachStairBoost = approachStairBoost });


        }
    }
}



