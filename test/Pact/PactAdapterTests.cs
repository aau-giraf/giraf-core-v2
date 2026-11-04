using PactNet.Verifier;

namespace giraf_core_v2.Tests.Pact;

public class PactAdapterTests
{
    // Todo replace URI with the one defined in the env
    // Todo figure out a way to fetch the contract file from foodplanner github repo per test run
    // Todo figure out the optimal way of naming URI's and change correspondingly. ATM: /legacy/app_name/...

    [Fact]
    public void Core_satisfies_the_foodplanner_contract()
    {
        var providerName = "core";
        var providerBaseUrl = new Uri("http://localhost:8081");
        var pactFile = new FileInfo(Path.Combine(AppContext.BaseDirectory, "../../../Pact/Contracts/core-foodplanner.json"));

        using var verifier = new PactVerifier(providerName, new PactVerifierConfig());

        // verifier does not care what the database is seeded with, as long as each individual entry matches the pattern defined by the consumer
        verifier
            .WithHttpEndpoint(providerBaseUrl)
            .WithFileSource(pactFile)
            .Verify();
    }
}