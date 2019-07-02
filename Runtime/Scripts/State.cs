using UnityEngine;

namespace DGTools.StateMachine
{
    public abstract class State : MonoBehaviour
    {
        //PRIVATE VARIABLES
        protected StateMachine owner;

        //PROPERTIES
        /// <summary>
        /// If a state is locked, it can't be changed
        /// </summary>
        public virtual bool isLocked { get; set; }

        //METHODS
        /// <summary>
        /// Called when StateMachine enters in this state
        /// </summary>
        public virtual void Enter()
        {
            owner = GetComponent<StateMachine>();
            AddListeners();
        }

        /// <summary>
        /// Called when StateMachine exits from this state
        /// </summary>
        public virtual void Exit()
        {
            RemoveListeners();
            DestroyImmediate(this, true);
        }

        /// <summary>
        /// Called when this state is destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            RemoveListeners();
        }

        //ABSTRACT METHODS
        protected abstract void AddListeners();

        protected abstract void RemoveListeners();
    }
}
