
using Moq;

namespace VaccineProjectTesting
{
    internal class Mock<T>
    {
        public Mock()
        {
        }

        internal object Setup(Func<object, object> value)
        {
            throw new NotImplementedException();
        }

        internal void Verify(Func<object, object> value, Func<Times> once)
        {
            throw new NotImplementedException();
        }
    }
}