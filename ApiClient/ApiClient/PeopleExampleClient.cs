using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClient
{
    public class PeopleExampleClient : BaseClient
    {
        public PeopleExampleClient(string baseUrl, string apiKey, string secret) : base(baseUrl, apiKey, secret)
        {
        }

        public async Task<IEnumerable<Person>> GetPeople()
        {
            var result = await base.GetAsync<PersonWrapper>("people");
            var people = result.People.ToList();
            while (result.PageCount > result.PageNumber)
            {
                var pageNumber = result.PageNumber++;
                result = await base.GetAsync<PersonWrapper>($"people?page_size=300");
                people.AddRange(result.People);
            }
            return people;
        }
    }
}
