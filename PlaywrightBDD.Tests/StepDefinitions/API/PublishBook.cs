using System.Collections;
using System.Security;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using static System.Environment;

namespace PlaywrightBDD.Tests.StepDefinitions.API;

[Binding]
public class PublishBook(PlaywrightApiDriver apiDriver)
{
    private string? _authenticationToken;
    private IAPIResponse? _response;

    [Given("I send request to get token for my next API call")]
    public async Task GivenISendRequestToGetTokenForMyNextApiCall()
    {
        const string json = """
                                { 
                                "user": { "email": "test@nai.com", "password": "12345678"}
                                }
                            """;
        var tokenResponse  = await apiDriver.Api.PostAsync("/api/users/login", new APIRequestContextOptions()
        {
            Data = json
        });
        Assert.That(tokenResponse.Status, Is.EqualTo(200), "Status response should be 200");
        var root = (await tokenResponse.JsonAsync())!.Value;
        _authenticationToken = root
            .GetProperty("user")
            .GetProperty("token")
            .GetString();
    }

    [Given("I am able to get the token which I am going to save in an environment variable")]
    public void GivenIAmAbleToGetTheTokenWhichIAmGoingToSaveInAnEnvironmentVariable()
    {
        try
        {
            SetEnvironmentVariable("AuthToken", _authenticationToken);
        }
        catch (ArgumentNullException exception)
        {
            Console.WriteLine(exception.Message, "Authentication Error. Token is null");
        }
    }

    [Given("I send request to my website to add my book with (.*) (.*) (.*) and (.*)")]
    public async Task SendRequestToMyWebsiteToAddMyBook(string title, string body, string description, string tagList )
    {
        if (_authenticationToken != null)
        {
            string? token = null;
            try
            {
                token = GetEnvironmentVariable("AuthToken");
            }
            catch (SecurityException exception)
            {
                Console.WriteLine(exception.Message, "Authentication Error. Token is null");
            }

            var randomizerFullName = RandomizerFactory.GetRandomizer(new FieldOptionsFullName());
            var fullName = randomizerFullName.Generate();

            var randomNumber = RandomizerFactory.GetRandomizer(new FieldOptionsInteger());
            var articleNumber = randomNumber.Generate();

            var randomTimeStamp = RandomizerFactory.GetRandomizer(new FieldOptionsDateTime());
            var date = randomTimeStamp.Generate();

            var payload = new
            {
                article = new
                {
                    title = title + articleNumber,
                    description = description + fullName,
                    body = body + date,
                    tagList = new ArrayList { tagList }
                }
            };
            
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + token }
            };
           
            _response = await apiDriver.Api.PostAsync("/api/articles/", new APIRequestContextOptions()
            {
                DataObject = payload,
                Headers = headers
            });
            Assert.That(_response.Status, Is.EqualTo(201), "Status response should be 201");
        }
    }

    [Then("I am able to confirm that it is successfully added to the website")]
    public async Task AbleToSeeThatItIsSuccessfullyAddedToTheWebsite()
    {
        if (_response != null)
        {
            var root = (await _response.JsonAsync())!.Value;
            var isFavourited = root
                .GetProperty("article")
                .GetProperty("favorited")
                .GetBoolean();
            Assert.That(isFavourited, Is.False);

            var imageUrl = root
                .GetProperty("article")
                .GetProperty("author")
                .GetProperty("image")
                .ToString();
            Assert.That(imageUrl, Is.EqualTo("https://conduit-api.bondaracademy.com/images/smiley-cyrus.jpeg"));
        }
    }
}