using System;
using System.Threading.Tasks;
using ASPNetUserService.Specs.Drivers;
using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ASPNetUserService.Specs.Steps
{
    [Binding]
    public class GetMessageSteps
    {
        private readonly UserServiceRequestDriver _userServiceApi;

        private TokenInfo? _tokenInfo;
        private string? _message;

        public GetMessageSteps(UserServiceRequestDriver userServiceApi)
        {
            _userServiceApi = userServiceApi;
        }

        [Given(@"there are no users registered in the system")]
        public void GivenThereAreNoUsersRegisteredInTheSystem()
        {
            // _userServiceApi.ClearRegistrations();
        }

        [Given(@"user is registered in the system")]
        public async Task GivenUserIsRegisteredInTheSystem(Table table)
        {
            UserInfo user = table.CreateInstance<UserInfo>();
            await _userServiceApi.RegisterUser(user);
        }

        [Given(@"user is logged in the system")]
        public async Task GivenUserIsLoggedInTheSystem(Table table)
        {
            UserInfo user = table.CreateInstance<UserInfo>();
            _tokenInfo = await _userServiceApi.LoginUser(user);
        }

        [When(@"user retrieves message from the system")]
        public async Task WhenUserRetrievesMessageFromTheSystem()
        {
            _message = await _userServiceApi.GetMessage(_tokenInfo);
        }

        [Then(@"message is retrieved containing user name")]
        public void ThenMessageIsRetrievedContainingUserName()
        {
            _message.Should().StartWith("user@mail.com");
        }
    }
}
