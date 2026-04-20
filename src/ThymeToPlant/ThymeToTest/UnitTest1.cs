using ThymeToPlant.Services;

namespace ThymeToTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Test]
    public void DummyTest()
    {
        var mathy = new FakeTestService();
        var result = mathy.AddMe(1, 2);

        Assert.That(result, Is.EqualTo(3));
    }


}
