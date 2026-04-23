using SharedData.Dto;

namespace Empen.Service.IService
{
    public interface IMailService
    {
        void sendMailOneAmountByPersonId(int personId, string title, string description, int reward_id_1, int? reward_id_2, int? reward_id_3, DateTime insertDate, DateTime updateDate);
        void sendMailByPersonId(int personId, string title, string description, int reward_id_1, int reward_id_1_amount, int? reward_id_2, int? reward_id_2_amount, int? reward_id_3, int? reward_id_3_amount, DateTime insertDate, DateTime updateDate);
        Task sendMailAllPersonId(string title, string description, int reward_id_1, int reward_id_1_amount, int? reward_id_2, int? reward_id_2_amount, int? reward_id_3, int? reward_id_3_amount, DateTime insertDate, DateTime updateDate);
        Task<ICollection<MailInfoDto>> getAllMail(int personId);
        //Task<ReceiveRewardListDto> takeAllMail(int personId);
        Task<ICollection<ObjectDisplayDto>> takeAllMailDisplay(int personId);
        Task<ICollection<ObjectDisplayDto>> takeMailById(int personId, int person_mail_id);
    }
}
