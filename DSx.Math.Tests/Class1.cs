using NUnit.Framework;

namespace DSx.Math.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Test()
        {
            
            var x = new Vector<float, float, float>(1, 0, 0);
            var y = new Vector<float, float, float>(0, 1, 0);
            var z = new Vector<float, float, float>(0, 0, 1);
            var v = new Vector<float, float, float>(4, 3, 8);
            var result = v.Project(x, y);
            
        }
    }
}