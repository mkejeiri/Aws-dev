using Xunit;

namespace MovieRank.Integration.Tests.Setup
{
    /*
     share the setup among other tests, this saves us having to do the test setup each time a test is run.
     xUnit gives us the ability to create a class as our entry point when we first run our tests.
     */
    [CollectionDefinition("api")]
    public class CollectionFixture :
        //when we run our tests, we want to the TestContext class to be run before we run our test
        ICollectionFixture<TestContext>
    {
    }
}
