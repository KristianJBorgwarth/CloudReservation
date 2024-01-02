using CloudReservation.DAL.Data;

namespace CloudReservation.IntegrationTest.IntegrationUtilities.TestFixtures;

[CollectionDefinition("TestCollection")]
public class ContainerCollectionFixture : ICollectionFixture<IntegrationTestFactory<Program, CloudReservationDbContext>>
{
    
}