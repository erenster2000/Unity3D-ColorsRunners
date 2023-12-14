using System;
using System.Collections.Generic;
using Data.UnityObject;
using Data.ValueObject;
using DG.Tweening;
using Signals;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Color = Enums.Color;

namespace Controllers.PlayerObjectsManager
{
    public class PlayerObjectsController : MonoBehaviour
    {
        #region Self Variables
        #region public Variables

        #endregion
        #region Serialized Variables
        
        [SerializeField] private List<Material> materials;
        [SerializeField] private Animator animator;

        #endregion
        #region Private Variables

        private Material _material;
        private ObjectData _objectData;
        private GameObject _execution;
        private Transform _oldTransform;

        #endregion
        #endregion

        private void Awake()
        {
            _objectData = GetObjectData();
        }

        private ObjectData GetObjectData(){return Resources.Load<SO_ObjectData>("Data/SO_ObjectData").ObjectData;}
        
        public void PlayerExecution(GameObject other){ _execution = other; }
        
        public void Comparison(GameObject door)
        {
            string removeName = " (Instance)";
            string materialName = door.GetComponent<Renderer>().material.name;
            int i = materialName.Length - removeName.Length;
            materialName = materialName.Remove(i, removeName.Length);
            Color index = (Color)Enum.Parse(typeof(Color), materialName);
            ColorChange(index);
        }
        
        public void ColorChange(Color color)
        {
            Material material = materials[(int)color];
            transform.GetChild(0).GetComponent<Renderer>().material = material;
        }

        public void MinigameControl()
        {
            //DataSignals();
            CoreGameSignals.Instance.minigameState?.Invoke("HelicopterMinigame");
            float distance = _objectData.distance;
            float i = _objectData.quantity;
            if (transform.position.x < 0)
            {
                transform.DOMoveX(-1.5f, .5f);
            }
            else
            {
                transform.DOMoveX(1.5f, .8f);
            }
            if (distance >= 7)
            {
                i = -.5f;
            }
            else if (distance <= 2.5f)
            {
                i = .5f;
            }
            distance += i;
            _objectData.distance = distance;
            _objectData.quantity = i;
            transform.DOMoveZ(transform.position.z + distance, 1).OnComplete(() => PlayerAnimation("StandingToCrouched"));
        }

        public void PlayExecution()
        {
            if (transform.GetChild(0).GetComponent<Renderer>().material.name != _execution.GetComponent<Renderer>().material.name ) // renkleri enum ataması yap ve onun üzerinden işlet.
            {
                Debug.Log(transform.GetChild(0).GetComponent<Renderer>().material);
                Debug.Log(_execution.GetComponent<Renderer>().material);
                PlayerAnimation("Dead");
            }
        }

        public void PlayerAnimation(string animation)
        {
            if (animation == "Runner")
            {
                animator.SetTrigger("Runner");
            }
            else if (animation == "Idle")
            {
                animator.SetTrigger("Idle");
            }
            else if (animation == "StandingToCrouched")
            {
                animator.SetTrigger("StandingToCrouched");
                _oldTransform = transform;
                transform.DOLocalMoveY(3.35f, .2f);
            }
            else if (animation == "Dead")
            {
                transform.DOMoveY(_oldTransform.position.y + .4f, .2f);
                animator.SetTrigger("Dead");
                transform.DOMoveY(_oldTransform.position.y + .1f, .5f).SetDelay(1);
            }
        }
    }
}