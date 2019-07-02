using UnityEngine;

namespace DGTools.StateMachine
{
    public abstract class StateWithParams<Tparam> : State
    {
        //VARIABLES
        /// <summary>
        /// That value is set by the <see cref="StateMachine"/> before <see cref="StateMachine.Transition(State)"/>
        /// </summary>
        [HideInInspector] public Tparam param;
    }
}
