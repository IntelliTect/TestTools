using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestCaseException : Exception
    {
        public TestCaseException() : base()
        {
        }

        public TestCaseException(string message) : base(message)
        {
        }

        public TestCaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TestCaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ResourceReferenceProperty = info.GetString("ResourceReferenceProperty");
        }

        public string ResourceReferenceProperty { get; set; } = "";

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null) throw new ArgumentNullException(nameof(info));
            info.AddValue("ResourceReferenceProperty", ResourceReferenceProperty);
            base.GetObjectData(info, context);
        }
    }
}
