using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using TestPostProject;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]
namespace TestPostMethod
{
    [TestClass]
    public class PostMethod
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";
        private static Uri GetUri(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<UserModel> cleanUpList = new List<UserModel>();

        [TestInitialize]
        public void TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURL($"{PetEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostProject()
        {
            UserModel petData = new UserModel()
            {
                Id = 1,
                Category = new Category()
                {
                    Id = 1,
                    Name = "Snoopy"
                },
                Name = "Snoopy",
                PhotoUrls = new List<string>
                {
                    "Pogi"
                },
                Tags = new List<Category>()
                {
                    new Category()
                {
                    Id = 1,
                    Name = "Snoopy"
                }
                },
                Status = "available"
            };
            // Send Post Request
            var postRestRequest = new RestRequest(GetUri(PetEndpoint)).AddJsonBody(petData);
            var restResponse = await restClient.ExecutePostAsync(postRestRequest);

            // Verify Post Request Status Code
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status Code is not equal to 200");

            // Send Get Request
            var getRestRquest = new RestRequest(GetUri($"{PetEndpoint}/{petData.Id}"));
            var getRestResponse = await restClient.ExecuteGetAsync<UserModel>(getRestRquest);

            Assert.AreEqual(HttpStatusCode.OK, getRestResponse.StatusCode, "Status Code is not equal to 200");
            Assert.AreEqual(petData.Name, getRestResponse.Data.Name, "Name did not Match");
            Assert.AreEqual(petData.Category.Name, getRestResponse.Data.Category.Name, "Category did not Match");
            Assert.AreEqual(petData.PhotoUrls[0], getRestResponse.Data.PhotoUrls[0], "PhotoUrls not found");
            Assert.AreEqual(petData.Tags[0].Name, getRestResponse.Data.Tags[0].Name, "tags not found");
            Assert.AreEqual(petData.Status, getRestResponse.Data.Status, "Status Code is not equal to 200");

            // CleanUp
            cleanUpList.Add(petData);
        }
    }
}