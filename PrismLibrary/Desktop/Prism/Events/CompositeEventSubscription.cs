using System;
using System.Threading;
using System.Windows.Threading;

namespace Microsoft.Practices.Prism.Events
{
    /// <summary>
    /// Extends <see cref="EventSubscription{TPayload}"/> to invoke the <see cref="EventSubscription{TPayload}.Action"/> delegate in the provided SynchronizationContext.
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class CompositeEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        SynchronizationContext _context = null;

        ///<summary>
        /// Creates a new instance of <see cref="BackgroundEventSubscription{TPayload}"/>.
        ///</summary>
        ///<param name="actionReference">A reference to a delegate of type <see cref="System.Action{TPayload}"/>.</param>
        ///<param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        ///<param name="context">The SynchronizationContext to use when executing the <paramref name="actionReference"/> delegate.</param>
        ///<exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <see paramref="filterReference"/> are <see langword="null" />.</exception>
        ///<exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="System.Action{TPayload}"/>,
        ///or the target of <paramref name="filterReference"/> is not of type <see cref="Predicate{TPayload}"/>.</exception>
        public CompositeEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference, SynchronizationContext context)
            : base(actionReference, filterReference)
        {
            _context = context;
        }

        /// <summary>
        /// Invokes the specified <see cref="System.Action{TPayload}"/> asynchronously in the specified <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass <paramref name="action"/> while invoking it.</param>
        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            if (_context != null)
            {
                SendOrPostCallback callback = new SendOrPostCallback(arg => action(argument));
                _context.Post(callback, null);
            }
            else
            {
                if(action != null)
                    action.Invoke(argument);
            }
        }
    }
}
