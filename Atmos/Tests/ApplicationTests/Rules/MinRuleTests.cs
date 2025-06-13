using Application.Rules;

namespace Tests.ApplicationTests.Rules;

public class MinRuleTests : BaseTest<MinRule>
{
    public MinRuleTests()
    {
        Subject = new MinRule();
    }
}
