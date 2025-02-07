namespace DefaultNamespace
{
    public class EngineEvent : BaseEvent
    {
        public EngineEventType engineEventType;
        public EngineEvent(EngineEventType type)
        {
            engineEventType = type;
        }
        
        public enum EngineEventType
        {
            StartBegin, StartFinish, StopBegin, StopFinish
        }
    }
}