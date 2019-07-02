using UnityEngine;
using System.Collections;
using System;

namespace DGTools.StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        //VARIABLES
        protected State _currentState;
        protected bool _inTransition = false;

        //PROPERTIES
        /// <summary>
        /// Returns the current state of the state machine
        /// </summary>
        public virtual State currentState
        {
            get { return _currentState; }
            set { Transition(value); }
        }

        /// <summary>
        /// StateMachine is locked if current state is locked, call <see cref="UnlockCurrentState()"/> to unlock it 
        /// </summary>
        public virtual bool isLocked
        {
            get
            {
                if (currentState != null && currentState.isLocked)
                {
                    Debug.Log(string.Format(
                        "State {0} is locked, unlock it to change state",
                        currentState.GetType()
                    ));
                    return true;
                }
                return false;
            }
        }       

        //METHODS
        /// <summary>
        /// Returns a State of type <typeparamref name="Tstate"/> attached to the StateMachine, it adds it to the StateMachine if not found
        /// </summary>
        /// <typeparam name="Tstate">Type of the state</typeparam>
        /// <returns>The State of type <typeparamref name="Tstate"/> attached to the machine</returns>
        public virtual Tstate GetState<Tstate>() where Tstate : State
        {
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
            target.param = param;
            return target;
        }

        /// <summary>
        /// Returns a State of given type attached to the StateMachine, it adds it to the StateMachine if not found
        /// </summary>
        /// <param name="type">Type of the state</param>
        /// <returns>The State of give Type attached to the machine</returns>
        public virtual State GetState(Type type)
        {
            if (type.IsSubclassOf(typeof(State)))
            {
                State target = (State)GetComponent(type);
                if (target == null)
                {
                    gameObject.AddComponent(type);
                    target = (State)GetComponent(type);
                }
                return target;
            }
            else
            {
                throw new Exception(type.ToString() + " does not inherit from state class");
            }
        }
        
        /// <summary>
        /// Destroys the current state
        /// </summary>
        public virtual void ClearState()
        {
            DestroyImmediate(_currentState, true);
            _currentState = null;
        }

        /// <summary>
        /// Makes a transition from the current State to a new State of type <typeparamref name="Tstate"/>
        /// </summary>
        /// <typeparam name="Tstate">The type of the State</typeparam>
        public virtual void ChangeState<Tstate>() where Tstate : State
        {
            if (isLocked) return;
            currentState = GetState<Tstate>();
        }

        /// <summary>
        /// Makes a transition from the current State to a new State of type <typeparamref name="Tstate"/> and set its param of type <typeparamref name="Tparam"/>
        /// </summary>
        /// <typeparam name="Tstate">The type of the State</typeparam>
        /// <typeparam name="Tparam">Type of state's param</typeparam>
        /// <param name="param">Value of state's param</param>
        public virtual void ChangeState<Tstate, Tparam>(Tparam param) where Tstate : StateWithParams<Tparam>
        {
            if (isLocked) return;
            currentState = GetState<Tstate, Tparam>(param);
        }

        /// <summary>
        ///  Makes a transition from the current State to a new State of give Type
        /// </summary>
        /// <param name="type">Type of the State</param>
        public virtual void ChangeState(Type type)
        {
            if (isLocked) return;
            try
            {
                currentState = GetState(type);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }
        }

        /// <summary>
        /// Performs the transition between current State and new State
        /// </summary>
        /// <param name="value">The new State</param>
        protected virtual void Transition(State value)
        {
            if (_currentState == value || _inTransition)
                return;
            _inTransition = true;
            if (_currentState != null)
                _currentState.Exit();
            _currentState = value;
            if (_currentState != null)
                _currentState.Enter();
            _inTransition = false;
        }

        /// <summary>
        /// Locks the current State
        /// </summary>
        public void LockCurrentState() {
            currentState.isLocked = true;
        }

        /// <summary>
        /// Unlocks the current State
        /// </summary>
        public void UnlockCurrentState()
        {
            currentState.isLocked = false;
        }
    }
}