using UnityEngine;

namespace DGTools.StateMachine
{
    public abstract class State : MonoBehaviour
    {
        #region Private Variables
        /// <summary>
        /// The <see cref="StateMachine"/> that owns this <see cref="State"/>
        /// </summary>
        protected StateMachine owner;
        #endregion

        #region Properties
        /// <summary>
        /// If a state is locked, it can't be changed
        /// </summary>
        public virtual bool IsLocked { get; set; }
        #endregion

        #region Public Methods
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
        #endregion

        #region Runtime Methods
        /// <summary>
        /// Called when this state is destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            RemoveListeners();
        }
        #endregion

        #region Abstract Methods
        protected abstract void AddListeners();

        protected abstract void RemoveListeners();
        #endregion
    }
}
