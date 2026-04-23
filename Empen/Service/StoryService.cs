using Empen.Data;
using Empen.Service.IService;
using Microsoft.EntityFrameworkCore;
using SharedData.Dto;
using SharedData.Models;

namespace Empen.Service
{
    //public class StoryService : IStoryService
    //{
    //    private readonly MasterDbContext _masterContext;
    //    private readonly PersonDbContext _personContext;
    //    private readonly IMailService _mailService;

    //    public StoryService(MasterDbContext masterContext, PersonDbContext personContext, IMailService mailService)
    //    {
    //        _masterContext = masterContext;
    //        _personContext = personContext;
    //        _mailService = mailService;
    //    }

    //    public async Task<ICollection<StoryGroupDto>> getAllStoryGroup()
    //    {
    //        DateTime now = DateTime.Now;

    //        var groups = await _masterContext.master_story_group
    //            .AsNoTracking()
    //            .OrderBy(s => s.show_order)
    //            .Where(s => s.start_date <= now && (s.end_date == null || s.end_date >= now))
    //            .Select(s => new StoryGroupDto()
    //            {
    //                story_group_id = s.story_group_id,
    //                group_name = s.group_name,
    //                story_type = s.story_type,
    //                main_character = s.main_character
    //            })
    //            .ToListAsync();

    //        return groups;
    //    }

    //    public async Task<bool> getStoryReward(int storyId, int personId)
    //    {
    //        DateTime now = DateTime.Now;

    //        bool alreadyReceived = await _personContext.person_story_history
    //            .AnyAsync(h => h.story_id == storyId && h.person_id == personId);

    //        if (alreadyReceived == true)
    //        {
    //            return false;
    //        }

    //        var rewards = await _masterContext.master_story
    //            .AsNoTracking()
    //            .Where(s => s.story_id == storyId)
    //            .Select(s => new { s.first_reward_id_1, s.first_reward_id_2, s.first_reward_id_3 })
    //            .FirstOrDefaultAsync();

    //        if (rewards == null)
    //        {
    //            return false;
    //        }

    //        using (var transaction = await _personContext.Database.BeginTransactionAsync())
    //        {
    //            try
    //            {
    //                if (rewards.first_reward_id_1 > 0)
    //                {
    //                    _mailService.sendMailOneAmountByPersonId(personId, Constant.STORY_REWARD_MAIL_TITLE, Constant.STORY_REWARD_MAIL_DES,
    //                        rewards.first_reward_id_1, null, null, now, now);
    //                }

    //                if (rewards.first_reward_id_2 > 0)
    //                {
    //                    _mailService.sendMailOneAmountByPersonId(personId, Constant.STORY_REWARD_MAIL_TITLE, Constant.STORY_REWARD_MAIL_DES,
    //                        rewards.first_reward_id_2, null, null, now, now);
    //                }

    //                if (rewards.first_reward_id_3 > 0)
    //                {
    //                    _mailService.sendMailOneAmountByPersonId(personId, Constant.STORY_REWARD_MAIL_TITLE, Constant.STORY_REWARD_MAIL_DES,
    //                        rewards.first_reward_id_3, null, null, now, now);
    //                }

    //                var history = new person_story_history
    //                {
    //                    person_id = personId,
    //                    story_id = storyId,
    //                    insert_date = now,
    //                    update_date = now
    //                };

    //                _personContext.person_story_history.Add(history);
    //                await _personContext.SaveChangesAsync();

    //                await transaction.CommitAsync();

    //                return true;
    //            }
    //            catch (Exception ex)
    //            {
    //                // TODO: 로깅
    //                await transaction.RollbackAsync();
    //                return false;
    //            }
    //        }
    //    }

    //    public async Task<ICollection<StoryDto>> getStorysByGroupId(int storyGroupId)
    //    {
    //        DateTime now = DateTime.Now;

    //        var storys = await _masterContext.master_story
    //            .AsNoTracking()
    //            .OrderBy(s => s.show_order)
    //            .Where(s => s.start_date <= now && (s.end_date == null || s.end_date >= now) && s.story_group_id == storyGroupId)
    //            .Select(s => new StoryDto()
    //            {
    //                story_id = s.story_id,
    //                story_group_id = s.story_group_id,
    //                story_name = s.story_name,
    //                first_reward_id_1 = s.first_reward_id_1,
    //                first_reward_id_2 = s.first_reward_id_2,
    //                first_reward_id_3 = s.first_reward_id_3,
    //            })
    //            .ToListAsync();

    //        return storys;
    //    }

    //    public async Task<ICollection<StoryScriptDto>> getStoryScriptsByStoryId(int storyId)
    //    {
    //        DateTime now = DateTime.Now;

    //        var scripts = await _masterContext.master_story_script
    //            .AsNoTracking()
    //            .OrderBy(s => s.show_order)
    //            .Where(s => s.start_date <= now && (s.end_date == null || s.end_date >= now) && s.story_id == storyId)
    //            .Select(s => new StoryScriptDto()
    //            {
    //                story_script_id = s.story_script_id,
    //                story_id = s.story_id,
    //                story_background_image = s.story_background_image,
    //                background_effect = s.background_effect,
    //                pos_1_char = s.pos_1_char,
    //                pos_1_char_motion = s.pos_1_char_motion,
    //                pos_2_char = s.pos_2_char,
    //                pos_2_char_motion = s.pos_2_char_motion,
    //                story_text = s.story_text,
    //                answer_1 = s.answer_1,
    //                answer_2 = s.answer_2,
    //                answer_3 = s.answer_3,
    //                answer_1_story_id = s.answer_1_story_id,
    //                answer_2_story_id = s.answer_2_story_id,
    //                answer_3_story_id = s.answer_3_story_id
    //            })
    //            .ToListAsync();

    //        return scripts;
    //    }
    //}
}
