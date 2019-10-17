using UnityEngine;
using System;
using UnityEngine.Events;

namespace DGTools.StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        #region Private Variables
        protected State currentState;
        #endregion

        #region Events
        [Serializable] public class StateMachineEvent : UnityEvent<StateMachine> { }
        [Header("Events")]
        public StateMachineEvent OnStateWillChange = new StateMachineEvent();
        public StateMachineEvent OnStateChanged = new StateMachineEvent();
        public StateMachineEvent OnStateUnlocked = new StateMachineEvent();
        #endregion

        #region Properties
        /// <summary>
        /// Returns the current state of the state machine
        /// </summary>
        public virtual State CurrentState
        {
            get { return currentState; }
            set { Transition(value); }
        }

        /// <summary>
        /// Override this to set default <see cref="State"/>, the <see cref="StateMachine"/> will enter this <see cref="State"/> on Awake
        /// </summary>
        public virtual Type DefaultState => null;

        /// <summary>
        /// Override this to set a <see cref="Type"/> contraint to <see cref="State"/>. A <see cref="State"/> should inherit from this type to be handled by this <see cref="StateMachine"/>
        /// </summary>
        public virtual Type StateTypeConstraint => typeof(State);

        /// <summary>
        /// StateMachine is locked if current state is locked, call <see cref="UnlockCurrentState()"/> to unlock it 
        /// </summary>
        public virtual bool IsLocked
        {
            get
            {
                if (CurrentState != null && CurrentState.IsLocked)
                {
                    Debug.Log(string.Format(
                        "State {0} is locked, unlock it to change state",
                        CurrentState.GetType()
                    ));
                    return true;
                }
                return false;
            }
        }

        public bool InTransition { get; protected set; } = false;
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a State of type <typeparamref name="Tstate"/> attached to the StateMachine, it adds it to the StateMachine if not found
        /// </summary>
        /// <typeparam name="Tstate">Type of the state</typeparam>
        /// <returns>The State of type <typeparamref name="Tstate"/> attached to the machine</returns>
        public virtual Tstate GetState<Tstate>() where Tstate : State
        {
            if (!IsValidStateType(typeof(Tstate))) return null;
            Tstate target = GetComponent<Tstate>();
            if (target == null)
                target = gameObject.AddComponent<Tstate>();
            return target;
        }

        /// <summary>
        /// Returns a State of type <typeparamref name="Tstate"/> attached to the StateMachine and set its <typeparamref name="Tparam"/>, it adds it to the StateMachine if not found
        /// </summary>
        /// <typeparam name="Tstate">Type of the state</typeparam>
        /// <typeparam name="Tparam">Type of state's param</typeparam>
        /// <param name="param">Value of state's param</param>
        /// <returns>The State of type <typeparamref name="Tstate"/> attached to the machine</returns>
        public virtual Tstate GetState<Tstate, Tparam>(Tparam param) where Tstate : StateWithParams<Tparam>
        {
            Tstate target = GetState<Tstate>();
            target.Params = param;
            return target;
        }

        /// <summary>
        /// Returns a State of given type attached to the StateMachine, it adds it to the StateMachine if not found
        /// </summary>
        /// <param name="type">Type of the state</param>
        /// <returns>The State of give Type attached to the machine</returns>
        public virtual State GetState(Type type)
        {
            if (!IsValidStateType(type)) return null;
            State target = (State)GetComponent(type);
            if (target == null)
            {
                gameObject.AddComponent(type);
                target = (State)GetComponent(type);
            }
            return target;
        }
        
        /// <summary>
        /// Destroys the current state
        /// </summary>
        public virtual void ClearState()
        {
            DestroyImmediate(CurrentState, true);
            CurrentState = null;
        }

        /// <summary>
        /// Makes a transition from the current State to a new State of type <typeparamref name="Tstate"/>
        /// </summary>
        /// <typeparam name="Tstate">The type of the State</typeparam>
        public virtual void ChangeState<Tstate>() where Tstate : State
        {
            if (IsLocked) return;
            CurrentState = GetState<Tstate>();
        }

        /// <summary>
        /// Makes a transition from the current State to a new State of type <typeparamref name="Tstate"/> and set its param of type <typeparamref name="Tparam"/>
        /// </summary>
        /// <typeparam name="Tstate">The type of the State</typeparam>
        /// <typeparam name="Tparam">Type of state's param</typeparam>
        /// <param name="param">Value of state's param</param>
        public virtual void ChangeState<Tstate, Tparam>(Tparam param) where Tstate : StateWithParams<Tparam>
        {
            if (IsLocked) return;
            CurrentState = GetState<Tstate, Tparam>(param);
        }

        /// <summary>
        ///  Makes a transition from the current State to a new State of give Type
        /// </summary>
        /// <param name="type">Type of the State</param>
        public virtual void ChangeState(Type type)
        {
            if (IsLocked) return;
            try
            {
                CurrentState = GetState(type);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }
        }

        /// <summary>
        /// Locks the current State
        /// </summary>
        public void LockCurrentState(UnityAction<StateMachine> onUnlock = null)
        {
            CurrentState.IsLocked = true;

            if(onUnlock != null)
            {
                OnStateUnlocked.AddListener(onUnlock);
            }
        }

        /// <summary>
        /// Unlocks the current State
        /// </summary>
        public void UnlockCurrentState(bool removeUnlockActions = true)
        {
            CurrentState.IsLocked = false;
            OnStateUnlocked.Invoke(this);

            if (removeUnlockActions)
                OnStateUnlocked.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Performs the transition between current State and new State
        /// </summary>
        /// <param name="value">The new State</param>
        protected virtual void Transition(State value)
        {
            if (currentState == value || InTransition)
                return;
            InTransition = true;
            OnStateWillChange.Invoke(this);
            if (currentState != null)
                currentState.Exit();
            currentState = value;
            if (currentState != null)
                currentState.Enter();
            else
                ClearState();
            InTransition = false;
            OnStateChanged.Invoke(this);
        }
        #endregion

        #region Runtime Methods
        private void Awake()
        {
            if(DefaultState != null)
            {
                ChangeState(DefaultState);
            }
        }

        protected virtual bool IsValidStateType(Type type)
        {
            if (!type.IsSubclassOf(typeof(State)))
                throw new Exception(string.Format("{0} should inherit from State to be used in a StateMachine", type.Name));

            if (!type.IsSubclassOf(StateTypeConstraint))
                throw new Exception(string.Format("{0} should inherit from {1} to be used in this StateMachine", type.Name, StateTypeConstraint.Name));

            return true;
        }
        #endregion
    }
}