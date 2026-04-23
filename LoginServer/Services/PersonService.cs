
using LoginServer.Models;
using LoginServer.Repositories;
using Microsoft.Data.SqlClient;
using ServerCore.Service;
using SharedData.Request;
using SharedData.Response;
using SharedData.Type;
using System.Net.Http;
using System.Net.Http.Json;

namespace LoginServer.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepo;
        private readonly ITimeService _timeService;
        private readonly HttpClient _httpClient;

        public PersonService(IPersonRepository personRepo, ITimeService timeService, HttpClient httpClient)
        {
            _personRepo = personRepo;
            _timeService = timeService;
            _httpClient = httpClient;
        }

        //public async Task<bool> checkExistsDisplayPersonIdAsync(int displayPersonId)
        //{
        //    return await _personRepo.existsDisplayIdAsync(displayPersonId);
        //}

        //public async Task<bool> checkExistsPersonIdAsync(int personId)
        //{
        //    var person = await _personRepo.getPersonByPersonIdAsync(personId);

        //    return person == null;
        //}

        public async Task<person?> findByDisplayPersonIdAsync(string provider, int displayPersonId)
        {
            return await _personRepo.getPersonByDisplayPersonIdAsync(provider, displayPersonId);
        }

        public async Task<person?> findByHashAsync(string provider, string hash)
        {
            return await _personRepo.getPersonByHashAsync(provider, hash);
        }

        public async Task<person?> findByPersonIdAsync(int personId)
        {
            return await _personRepo.getPersonByPersonIdAsync(personId);
        }

        public async Task<person?> registerPersonAsync(string loginProvider, string personHash, string email)
        {
            var now = await _timeService.getNowAsync();

            int maxRetries = 5;
            int currentRetry = 0;

            person person = null;
            string newEmail = email;

            while (currentRetry < maxRetries)
            {
                try
                {
                    //string newDisplayId = await generateUniqueDisplayIdAsync();
                    string newDisplayId = Random.Shared.Next(100000000, 999999999).ToString();

                    if (newEmail.Equals("Guest"))
                    {
                        newEmail = "Guest@" + newDisplayId;
                    }

                    person = new person
                    {
                        display_person_id = int.Parse(newDisplayId),
                        login_provider = loginProvider,
                        person_hash = personHash,
                        email = newEmail,
                        insert_date = now,
                        update_date = now
                    };

                    int newPersonId = await _personRepo.createPersonAsync(person);
                    person.person_id = newPersonId;

                    break;
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    currentRetry++;
                    if (currentRetry >= maxRetries)
                    {
                        throw new Exception("중복이 5번이상 발생했습니다.");
                    }
                }
                catch (Exception ex)
                {
                    if (person != null && person.person_id > 0)
                    {
                        await _personRepo.deletePersonAsync(person.person_id);
                    }
                    return null;
                }
            }

            CreatePersonRequest request = new CreatePersonRequest
            {
                person_id = person.person_id,
                display_person_id = person.display_person_id,
                person_hash = person.person_hash,
                email = newEmail
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"/auth/create", request);

                bool isSuccess = response.IsSuccessStatusCode;

                if (isSuccess)
                {
                    var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                    if (result == null || result.Result != ErrorCode.Success)
                    {
                        isSuccess = false;
                    }
                }

                if (!isSuccess)
                {
                    await _personRepo.deletePersonAsync(person.person_id);
                    return null;
                }
            }
            catch (Exception ex)
            {
                await _personRepo.deletePersonAsync(person.person_id);
                return null;
            }

            return person;
        }
    }
}
