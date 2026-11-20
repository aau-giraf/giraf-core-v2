using PactNet.Verifier;

namespace giraf_core_v2.Tests.Pact;

public class PactAdapterTests
{
    // URI points at the contracts on foodplanner-api's staging branch. Copy the url into a browser to see the files contents.  
    private readonly Uri _pactUri = new("https://raw.githubusercontent.com/aau-giraf/foodplanner-api/refs/heads/staging/contracts/foodplanner-api-core.json");

    // Must match the host port published for the server in compose.yaml.
    private readonly Uri _providerBaseUrl = new("http://localhost:8080");

    [Fact]
    public void Core_satisfies_the_foodplanner_contract()
    {
        var providerName = "core";
        var verifier = new PactVerifier(providerName, new PactVerifierConfig());

        try // is wrapped in a try-finally. There should be no catch since that would make the test fail with an exception. 
        {
            verifier
                .WithHttpEndpoint(_providerBaseUrl)
                .WithUriSource(_pactUri)
                .Verify();
        }
        finally // PactVerifier implements IDisposable, which means it holds something that must be released explicitly.
        {
            verifier.Dispose();
        }
    }
}