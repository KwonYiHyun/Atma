using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using SharedData.Dto;
using SharedData.Models;
using SharedData.Type;
using System;
using System.Collections.Generic;

namespace Empen.Service
{
    //public class FriendService : IFriendService
    //{
    //    private MasterDbContext _masterContext;
    //    private PersonDbContext _personContext;

    //    public FriendService(MasterDbContext masterContext, PersonDbContext personContext)
    //    {
    //        _masterContext = masterContext;
    //        _personContext = personContext;
    //    }

    //    private async Task<List<FriendInfoDto>> createFriendInfoListAsync(Dictionary<int, DateTime> targetMap, HashSet<int> mutualIds)
    //    {
    //        if (targetMap.Count == 0)
    //        {
    //            return new List<FriendInfoDto>();
    //        }

    //        var targetIds = targetMap.Keys;

    //        var friendStatuses = await _personContext.person_status
    //            .AsNoTracking()
    //            .Where(p => targetIds.Contains(p.person_id))
    //            .ToListAsync();

    //        var leaderCardIds = friendStatuses
    //            .Select(p => p.leader_person_character_id)
    //            .Where(id => id > 0)
    //            .Distinct()
    //            .ToList();

    //        var loginMap = await _personContext.person_login
    //            .AsNoTracking()
    //            .Where(l => targetIds.Contains(l.person_id))
    //            .ToDictionaryAsync(l => l.person_id, l => l.update_date);

    //        var characterMap = await _masterContext.master_character
    //            .AsNoTracking()
    //            .Where(c => leaderCardIds.Contains(c.character_id))
    //            .ToDictionaryAsync(c => c.character_id, c => c.character_name);

    //        List<FriendInfoDto> resultList = new List<FriendInfoDto>();

    //        foreach (var friend in friendStatuses)
    //        {
    //            string charName = "Unknown";
    //            if (friend.leader_person_character_id > 0 && characterMap.TryGetValue(friend.leader_person_character_id, out var name))
    //            {
    //                charName = name;
    //            }

    //            DateTime last_login_date = default(DateTime);
    //            if (loginMap.TryGetValue(friend.person_id, out var date))
    //            {
    //                last_login_date = date;
    //            }

    //            bool isMutual = mutualIds != null && mutualIds.Contains(friend.person_id);

    //            resultList.Add(new FriendInfoDto
    //            {
    //                person_id = friend.person_id,
    //                person_name = friend.person_name,
    //                level = friend.level,
    //                last_login_date = last_login_date,
    //                friend_date = targetMap.ContainsKey(friend.person_id) ? targetMap[friend.person_id] : default,
    //                introduce = friend.introduce,
    //                leader_person_character_id = friend.leader_person_character_id,
    //                leader_character_name = charName,
    //                each_other = isMutual
    //            });
    //        }

    //        return resultList;
    //    }

    //    public async Task<ICollection<FriendInfoDto>> getAllFollowingList(int personId)
    //    {
    //        var following = await _personContext.person_friend
    //            .AsNoTracking()
    //            .Where(f => f.person_id == personId)
    //            .Select(f => new { Id = f.friend_person_id, Date = f.insert_date })
    //            .ToListAsync();

    //        var followingMap = following.ToDictionary(f => f.Id, f => f.Date);
    //        var targetIds = followingMap.Keys;

    //        if (!targetIds.Any())
    //        {
    //            return new List<FriendInfoDto>();
    //        }

    //        var mutualIds = await _personContext.person_friend
    //            .AsNoTracking()
    //            .Where(f => f.friend_person_id == personId && targetIds.Contains(f.person_id))
    //            .Select(f => f.person_id)
    //            .ToHashSetAsync();

    //        return await createFriendInfoListAsync(followingMap, mutualIds);
    //    }
        
    //    public async Task<ICollection<FriendInfoDto>> getAllFollowerList(int personId)
    //    {
    //        var follower = await _personContext.person_friend
    //            .AsNoTracking()
    //            .Where(f => f.friend_person_id == personId)
    //            .Select(f => new { Id = f.person_id, Date = f.insert_date })
    //            .ToListAsync();

    //        var followerMap = follower.ToDictionary(f => f.Id, f => f.Date);
    //        var targetIds = followerMap.Keys;

    //        if (!targetIds.Any())
    //        {
    //            return new List<FriendInfoDto>();
    //        }

    //        var mutualIds = await _personContext.person_friend
    //            .AsNoTracking()
    //            .Where(f => f.person_id == personId && targetIds.Contains(f.friend_person_id))
    //            .Select(f => f.friend_person_id)
    //            .ToHashSetAsync();

    //        return await createFriendInfoListAsync(followerMap, mutualIds);
    //    }

    //    public async Task<ICollection<FriendInfoDto>> getRecommendList(int myId)
    //    {
    //        HashSet<int> excludeIds = new HashSet<int>();

    //        excludeIds.Add(myId);

    //        var friends1 = await _personContext.person_friend
    //            .AsNoTracking()
    //            .Where(f => f.person_id == myId)
    //            .Select(f => f.friend_person_id)
    //            .ToListAsync();

    //        var friends2 = await _personContext.person_friend
    //            .AsNoTracking()
    //            .Where(f => f.friend_person_id == myId)
    //            .Select(f => f.person_id)
    //            .ToListAsync();

    //        excludeIds.UnionWith(friends1);
    //        excludeIds.UnionWith(friends2);

    //        // 추천유저
    //        var recommendations = await _personContext.person_login
    //            .AsNoTracking()
    //            .Where(r => !excludeIds.Contains(r.person_id))
    //            .OrderByDescending(r => r.update_date)
    //            .Select(r => r.person_id)
    //            .Take(20)
    //            .ToListAsync();

    //        var recommendationStatus = await _personContext.person_status
    //            .AsNoTracking()
    //            .Where(p => recommendations.Contains(p.person_id))
    //            .ToListAsync();

    //        if (recommendations.Count == 0)
    //        {
    //            return new List<FriendInfoDto>();
    //        }

    //        var dummyMap = recommendations.ToDictionary(id => id, id => DateTime.MinValue);

    //        var emptyMutualSet = new HashSet<int>();

    //        return await createFriendInfoListAsync(dummyMap, emptyMutualSet);
    //    }

    //    public async Task<ErrorCode> requestFollowing(int personId, int friendId)
    //    {
    //        if (personId == friendId)
    //        {
    //            return ErrorCode.MyselfError;
    //        }

    //        bool exists = await _personContext.person_friend
    //            .AnyAsync(f => (f.person_id == personId && f.friend_person_id == friendId));

    //        if (exists == true)
    //        {
    //            return ErrorCode.DuplicationFriend;
    //        }

    //        DateTime now = DateTime.Now;

    //        person_friend friend = new person_friend()
    //        {
    //            person_id = personId,
    //            friend_person_id = friendId,
    //            type = 0,
    //            insert_date = now,
    //            update_date = now
    //        };

    //        _personContext.Add(friend);

    //        try
    //        {
    //            await _personContext.SaveChangesAsync();
    //            return ErrorCode.Success;
    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            // DB Concurrency Issue
    //            // TODO: Logging
    //            return ErrorCode.DbUpdateConcurrencyException;
    //        }
    //        catch (Exception ex)
    //        {
    //            // TODO: Logging
    //            return ErrorCode.TransactionFailed;
    //        }
    //    }

    //    public async Task<ErrorCode> requestFollowingById(int personId, int displayId)
    //    {
    //        int friendId = await _personContext.person
    //            .AsNoTracking()
    //            .Where(p => p.display_person_id == displayId)
    //            .Select(p => p.person_id)
    //            .FirstOrDefaultAsync();

    //        if (friendId == 0)
    //        {
    //            return ErrorCode.DataNotFound;
    //        }

    //        return await requestFollowing(personId, friendId);
    //    }
        
    //    public async Task<ErrorCode> deleteFollower(int personId, int friendId)
    //    {
    //        if (personId == friendId)
    //        {
    //            return ErrorCode.MyselfError;
    //        }

    //        bool exists = await _personContext.person_friend
    //            .AnyAsync(f => (f.person_id == friendId && f.friend_person_id == personId));

    //        if (exists == false)
    //        {
    //            return ErrorCode.DataNotFound;
    //        }

    //        int deletedCount = await _personContext.person_friend
    //            .Where(f => (f.person_id == friendId && f.friend_person_id == personId))
    //            .ExecuteDeleteAsync();

    //        if (deletedCount > 0)
    //        {
    //            return ErrorCode.Success;
    //        }
    //        else
    //        {
    //            return ErrorCode.DataNotFound;
    //        }
    //    }

    //    public async Task<ErrorCode> deleteFollowing(int personId, int friendId)
    //    {
    //        if (personId == friendId)
    //        {
    //            return ErrorCode.MyselfError;
    //        }

    //        bool exists = await _personContext.person_friend
    //            .AnyAsync(f => (f.person_id == personId && f.friend_person_id == friendId));

    //        if (exists == false)
    //        {
    //            return ErrorCode.DataNotFound;
    //        }

    //        int deletedCount = await _personContext.person_friend
    //            .Where(f => (f.person_id == personId && f.friend_person_id == friendId))
    //            .ExecuteDeleteAsync();

    //        if (deletedCount > 0)
    //        {
    //            return ErrorCode.Success;
    //        }
    //        else
    //        {
    //            return ErrorCode.DataNotFound;
    //        }
    //    }
    //}
}
