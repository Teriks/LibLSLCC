namespace LibLSLCC.Settings
{
    public class SettingsPropertyChangingEventArgs<TSetting>
    {
        public SettingsPropertyChangingEventArgs(TSetting propertyOwner, object subscriber, string propertyName)
        {
            PropertyOwner = propertyOwner;
            Subscriber = subscriber;
            PropertyName = propertyName;
        }

        public TSetting PropertyOwner { get; private set; }

        public object Subscriber { get; private set; }

        public string PropertyName { get; private set; }
    }
}