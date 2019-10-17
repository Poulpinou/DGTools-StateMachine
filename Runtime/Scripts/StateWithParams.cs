using UnityEngine;

namespace DGTools.StateMachine
{
    public abstract class StateWithParams<Tparams> : State
    {
        #region Properties
        /// <summary>
        /// That value is set by the <see cref="StateMachine"/> before <see cref="StateMachine.Transition(State)"/>
        /// </summary>
        public virtual Tparams Params { get; internal set; }
        #endregion
    }
}
