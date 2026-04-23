using SharedData.Dto;
using SharedData.Dto.Admin;
using SharedData.Request;
using SharedData.Response;
using SharedData.Type;

namespace AdminTool.API
{
    public class GameServerClient
    {
        private readonly HttpClient _httpClient;

        public GameServerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> giveItemAsync(int personId, int itemId, int count)
        {
            var request = new GiveItemRequest(personId, itemId, count);

            var response = await _httpClient.PostAsJsonAsync($"/admin/item/give", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                return result?.Result == ErrorCode.Success;
            }

            return false;
        }

        #region MasterItem
        public async Task<List<MasterItemDto>> getItems()
        {
            List<MasterItemDto> emptyResult = new List<MasterItemDto>();

            var response = await _httpClient.PostAsync($"/admin/item/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterItemDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editItemAsync(MasterItemDto item)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/item/edit", item);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addItemAsync(MasterItemDto item)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/item/add", item);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterGacha
        public async Task<List<MasterGachaDto>> getGachas()
        {
            List<MasterGachaDto> emptyResult = new List<MasterGachaDto>();

            var response = await _httpClient.PostAsync($"/admin/gacha/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterGachaDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editGachaAsync(MasterGachaDto item)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/gacha/edit", item);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addGachaAsync(MasterGachaDto item)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/gacha/add", item);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterBanner
        public async Task<List<MasterBannerDto>> getBanners()
        {
            List<MasterBannerDto> emptyResult = new List<MasterBannerDto>();

            var response = await _httpClient.PostAsync($"/admin/banner/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterBannerDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editBannerAsync(MasterBannerDto banner)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/banner/edit", banner);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addBannerAsync(MasterBannerDto banner)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/banner/add", banner);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterReward
        public async Task<List<MasterRewardDto>> getRewards()
        {
            List<MasterRewardDto> emptyResult = new List<MasterRewardDto>();

            var response = await _httpClient.PostAsync($"/admin/reward/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterRewardDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editRewardAsync(MasterRewardDto banner)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/reward/edit", banner);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addRewardAsync(MasterRewardDto banner)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/reward/add", banner);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterCharacter
        public async Task<List<MasterCharacterDto>> getCharacters()
        {
            List<MasterCharacterDto> emptyResult = new List<MasterCharacterDto>();

            var response = await _httpClient.PostAsync($"/admin/character/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterCharacterDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editCharacterAsync(MasterCharacterDto character)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/character/edit", character);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addCharacterAsync(MasterCharacterDto character)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/character/add", character);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterCharacterLevel
        public async Task<List<MasterCharacterLevelDto>> getCharacterLevels()
        {
            List<MasterCharacterLevelDto> emptyResult = new List<MasterCharacterLevelDto>();

            var response = await _httpClient.PostAsync($"/admin/characterlevel/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterCharacterLevelDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editCharacterLevelAsync(MasterCharacterLevelDto character)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/characterlevel/edit", character);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addCharacterLevelAsync(MasterCharacterLevelDto character)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/characterlevel/add", character);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterCharacterGrade
        public async Task<List<MasterCharacterGradeDto>> getCharacterGrades()
        {
            List<MasterCharacterGradeDto> emptyResult = new List<MasterCharacterGradeDto>();

            var response = await _httpClient.PostAsync($"/admin/charactergrade/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterCharacterGradeDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editCharacterGradeAsync(MasterCharacterGradeDto character)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/charactergrade/edit", character);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addCharacterGradeAsync(MasterCharacterGradeDto character)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/charactergrade/add", character);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterNotice
        public async Task<List<MasterNoticeDto>> getNotices()
        {
            List<MasterNoticeDto> emptyResult = new List<MasterNoticeDto>();

            var response = await _httpClient.PostAsync($"/admin/notice/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterNoticeDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editNoticeAsync(MasterNoticeDto character)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/notice/edit", character);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addNoticeAsync(MasterNoticeDto character)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/notice/add", character);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterProduct
        public async Task<List<MasterProductDto>> getProducts()
        {
            List<MasterProductDto> emptyResult = new List<MasterProductDto>();

            var response = await _httpClient.PostAsync($"/admin/product/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterProductDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editProductAsync(MasterProductDto product)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product/edit", product);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addProductAsync(MasterProductDto product)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product/add", product);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterProductSet
        public async Task<List<MasterProductSetDto>> getProductSets()
        {
            List<MasterProductSetDto> emptyResult = new List<MasterProductSetDto>();

            var response = await _httpClient.PostAsync($"/admin/product_set/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterProductSetDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editProductSetAsync(MasterProductSetDto productSet)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product_set/edit", productSet);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addProductSetAsync(MasterProductSetDto productSet)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product_set/add", productSet);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterProductSetPiece
        public async Task<List<MasterProductSetPieceDto>> getProductSetPieces()
        {
            List<MasterProductSetPieceDto> emptyResult = new List<MasterProductSetPieceDto>();

            var response = await _httpClient.PostAsync($"/admin/product_set/piece/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterProductSetPieceDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editProductSetPieceAsync(MasterProductSetPieceDto productSetPiece)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product_set/piece/edit", productSetPiece);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addProductSetPieceAsync(MasterProductSetPieceDto productSetPiece)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product_set/piece/add", productSetPiece);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterProductSetPrism
        public async Task<List<MasterProductSetPrismDto>> getProductSetPrisms()
        {
            List<MasterProductSetPrismDto> emptyResult = new List<MasterProductSetPrismDto>();

            var response = await _httpClient.PostAsync($"/admin/product_set/prism/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterProductSetPrismDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editProductSetPrismAsync(MasterProductSetPrismDto productSetPrism)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product_set/prism/edit", productSetPrism);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addProductSetPrismAsync(MasterProductSetPrismDto productSetPrism)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product_set/prism/add", productSetPrism);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterProductSetToken
        public async Task<List<MasterProductSetTokenDto>> getProductSetTokens()
        {
            List<MasterProductSetTokenDto> emptyResult = new List<MasterProductSetTokenDto>();

            var response = await _httpClient.PostAsync($"/admin/product_set/token/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterProductSetTokenDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editProductSetTokenAsync(MasterProductSetTokenDto productset)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product_set/token/edit", productset);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addProductSetTokenAsync(MasterProductSetTokenDto productset)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/product_set/token/add", productset);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterLoginBonus
        public async Task<List<MasterDailyLoginBonusDto>> getLoginBonuses()
        {
            List<MasterDailyLoginBonusDto> emptyResult = new List<MasterDailyLoginBonusDto>();

            var response = await _httpClient.PostAsync($"/admin/loginbonus/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterDailyLoginBonusDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editLoginBonusAsync(MasterDailyLoginBonusDto loginbonus)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/loginbonus/edit", loginbonus);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addLoginBonusAsync(MasterDailyLoginBonusDto loginbonus)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/loginbonus/add", loginbonus);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterLoginBonusDay
        public async Task<List<MasterDailyLoginBonusDayDto>> getLoginBonusDays()
        {
            List<MasterDailyLoginBonusDayDto> emptyResult = new List<MasterDailyLoginBonusDayDto>();

            var response = await _httpClient.PostAsync($"/admin/loginbonusday/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterDailyLoginBonusDayDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editLoginBonusDayAsync(MasterDailyLoginBonusDayDto day)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/loginbonusday/edit", day);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addLoginBonusDayAsync(MasterDailyLoginBonusDayDto day)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/loginbonusday/add", day);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterAchievement
        public async Task<List<MasterAchievementDto>> getAchievements()
        {
            List<MasterAchievementDto> emptyResult = new List<MasterAchievementDto>();

            var response = await _httpClient.PostAsync($"/admin/achievement/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterAchievementDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editAchievementAsync(MasterAchievementDto achievement)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/achievement/edit", achievement);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addAchievementAsync(MasterAchievementDto achievement)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/achievement/add", achievement);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion

        #region MasterMail
        public async Task<List<MasterMailDto>> getMails()
        {
            List<MasterMailDto> emptyResult = new List<MasterMailDto>();

            var response = await _httpClient.PostAsync($"/admin/mail/get/all", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<MasterMailDto>>>();
                if (result.Result != ErrorCode.Success)
                {
                    return emptyResult;
                }
                else
                {
                    return result.Data;
                }
            }

            return emptyResult;
        }

        public async Task<(bool, string)> editMailAsync(MasterMailDto mail)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/mail/edit", mail);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> addMailAsync(MasterMailDto mail)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/mail/add", mail);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                else
                {
                    return (result.Result == ErrorCode.Success, result.Data);
                }
            }
            return (false, "통신 에러");
        }
        #endregion


        #region PersonStatus
        public async Task<PersonStatusDto> getPersonStatusAsync(int personId)
        {
            var response = await _httpClient.PostAsync($"/admin/person_status/get/{personId}", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<PersonStatusDto>>();
                if (result != null && result.Result == ErrorCode.Success)
                {
                    return result.Data;
                }
            }
            return null;
        }

        public async Task<(bool, string)> editPersonStatusAsync(PersonStatusDto status)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/person_status/edit", status);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                return result != null ? (result.Result == ErrorCode.Success, result.Data) : (false, "통신 결과 에러");
            }
            return (false, "통신 에러");
        }
        #endregion

        #region PersonItem
        public async Task<List<PersonItemDto>> getPersonItemsAsync(int personId)
        {
            var response = await _httpClient.PostAsync($"/admin/person_item/get/{personId}", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<PersonItemDto>>>();
                if (result != null && result.Result == ErrorCode.Success) return result.Data;
            }
            return new List<PersonItemDto>();
        }

        public async Task<(bool, string)> editPersonItemAsync(PersonItemDto item)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/person_item/edit", item);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                return result != null ? (result.Result == ErrorCode.Success, result.Data) : (false, "통신 결과 에러");
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> deletePersonItemAsync(int personItemId)
        {
            var response = await _httpClient.PostAsync($"/admin/person_item/delete/{personItemId}", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                return result != null ? (result.Result == ErrorCode.Success, result.Data) : (false, "통신 결과 에러");
            }
            return (false, "통신 에러");
        }
        #endregion

        #region PersonItemHistory
        public async Task<List<PersonItemHistoryDto>> getPersonItemHistorysAsync(int personId)
        {
            var response = await _httpClient.PostAsync($"/admin/person_item_history/get/{personId}", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<PersonItemHistoryDto>>>();
                if (result != null && result.Result == ErrorCode.Success) return result.Data;
            }
            return new List<PersonItemHistoryDto>();
        }
        #endregion

        #region PersonMail
        public async Task<List<PersonMailDto>> getPersonMailsAsync(int personId)
        {
            var response = await _httpClient.PostAsync($"/admin/person_mail/get/{personId}", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<PersonMailDto>>>();
                if (result != null && result.Result == ErrorCode.Success) return result.Data;
            }
            return new List<PersonMailDto>(); // 에러 시 빈 리스트 반환
        }

        public async Task<(bool, string)> editPersonMailAsync(PersonMailDto mail)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/person_mail/edit", mail);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                return result != null ? (result.Result == ErrorCode.Success, result.Data) : (false, "통신 결과 에러");
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> deletePersonMailAsync(int personMailId)
        {
            var response = await _httpClient.PostAsync($"/admin/person_mail/delete/{personMailId}", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                return result != null ? (result.Result == ErrorCode.Success, result.Data) : (false, "통신 결과 에러");
            }
            return (false, "통신 에러");
        }
        #endregion

        #region PersonGacha
        // 특정 유저의 가챠 기록 조회
        public async Task<List<PersonGachaDto>> getPersonGachasAsync(int personId)
        {
            var response = await _httpClient.PostAsync($"/admin/person_gacha/get/{personId}", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<List<PersonGachaDto>>>();
                if (result != null && result.Result == ErrorCode.Success)
                {
                    return result.Data;
                }
            }

            // 에러나 데이터가 없을 경우 빈 리스트 반환 방어 코드
            return new List<PersonGachaDto>();
        }

        // 특정 유저의 가챠 횟수 수정
        public async Task<(bool, string)> editPersonGachaAsync(PersonGachaDto gacha)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/person_gacha/edit", gacha);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러 (데이터 없음)");
                }

                return (result.Result == ErrorCode.Success, result.Data ?? "성공");
            }
            return (false, $"통신 에러: {response.StatusCode}");
        }

        public async Task<(bool, string)> deletePersonGachaAsync(int personGachaId)
        {
            // 삭제의 경우 ID값만 URL 파라미터로 넘겨 처리합니다.
            var response = await _httpClient.PostAsync($"/admin/person_gacha/delete/{personGachaId}", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러 (데이터 없음)");
                }

                return (result.Result == ErrorCode.Success, result.Data ?? "성공");
            }
            return (false, $"통신 에러: {response.StatusCode}");
        }
        #endregion

        #region AdminManagement
        public async Task<(bool, string)> clearCacheAsync(string targetType)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/manage/cache/clear", targetType);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                return (result.Result == ErrorCode.Success, result.Data ?? "성공");
            }
            return (false, "통신 에러");
        }

        public async Task<long> getServerTimeOffsetAsync()
        {
            var response = await _httpClient.PostAsync("/admin/manage/time/get", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<long>>();
                if (result != null && result.Result == ErrorCode.Success)
                {
                    return result.Data;
                }
            }
            return 0;
        }

        public async Task<(bool, string)> setServerTimeOffsetAsync(long offsetTicks)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/manage/time/set", offsetTicks);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                return (result.Result == ErrorCode.Success, result.Data ?? "성공");
            }
            return (false, "통신 에러");
        }

        public async Task<bool> getMaintenanceStatusAsync()
        {
            var response = await _httpClient.PostAsync("/admin/manage/maintenance/get", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<bool>>();
                if (result != null && result.Result == ErrorCode.Success)
                {
                    return result.Data;
                }
            }
            return false;
        }

        public async Task<(bool, string)> setMaintenanceStatusAsync(bool isEnable)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/manage/maintenance/set", isEnable);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                return (result.Result == ErrorCode.Success, result.Data ?? "성공");
            }
            return (false, "통신 에러");
        }

        public async Task<(bool, string)> sendRewardMailAsync(PersonMailDto mail)
        {
            var response = await _httpClient.PostAsJsonAsync("/admin/manage/mail/send", mail);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GameResponse<string>>();
                if (result == null)
                {
                    return (false, "통신 결과 에러");
                }
                return (result.Result == ErrorCode.Success, result.Data ?? "성공");
            }
            return (false, "통신 에러");
        }
        #endregion
    }
}
