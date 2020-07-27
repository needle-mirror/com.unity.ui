namespace UnityEngine.UIElements
{
    class RuntimeEventInterpreter : EventInterpreter
    {
        internal new static readonly RuntimeEventInterpreter s_Instance = new RuntimeEventInterpreter();

        public override bool IsActivationEvent(EventBase evt)
        {
            return evt.eventTypeId == NavigationSubmitEvent.TypeId();
        }

        public override bool IsCancellationEvent(EventBase evt)
        {
            return evt.eventTypeId == NavigationCancelEvent.TypeId();
        }

        public override bool IsNavigationEvent(EventBase evt, out NavigationDirection direction)
        {
            if (evt.eventTypeId == NavigationMoveEvent.TypeId())
            {
                NavigationDirection GetDirection(NavigationMoveEvent.Direction moveDirection)
                {
                    switch (moveDirection)
                    {
                        case NavigationMoveEvent.Direction.Left: return NavigationDirection.Left;
                        case NavigationMoveEvent.Direction.Right: return NavigationDirection.Right;
                        case NavigationMoveEvent.Direction.Up: return NavigationDirection.Up;
                        case NavigationMoveEvent.Direction.Down: return NavigationDirection.Down;
                    }
                    return NavigationDirection.None;
                }

                direction = GetDirection(((NavigationMoveEvent)evt).direction);
                return direction != NavigationDirection.None;
            }

            if (base.IsNavigationEvent(evt, out direction))
            {
                // Don't send navigation directions through keyboard events, since they're going to be handled
                // by navigation events already.
                if (direction == NavigationDirection.Left || direction == NavigationDirection.Right ||
                    direction == NavigationDirection.Down || direction == NavigationDirection.Up)
                    direction = NavigationDirection.None;
            }

            return direction != NavigationDirection.None;
        }
    }
}
