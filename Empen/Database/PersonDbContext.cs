using Microsoft.EntityFrameworkCore;
using SharedData.Models;

namespace Empen
{
    public class PersonDbContext : DbContext
    {
        //public DbSet<person> person { get; set; }
        public DbSet<person_status> person_status { get; set; }
        public DbSet<person_item> person_item { get; set; }
        public DbSet<person_item_history> person_item_history { get; set; }
        public DbSet<person_loginbonus> person_loginbonus { get; set; }
        public DbSet<person_login> person_login { get; set; }
        public DbSet<person_mail> person_mail { get; set; }
        public DbSet<person_character> person_character { get; set; }
        //public DbSet<person_deck> person_deck { get; set; }
        //public DbSet<person_room> person_room { get; set; }
        //public DbSet<person_social> person_social { get; set; }
        //public DbSet<person_friend> person_friend { get; set; }
        public DbSet<person_gacha> person_gacha { get; set; }
        public DbSet<person_product> person_product { get; set; }
        public DbSet<person_product_set_prism> person_product_set_prism { get; set; }
        public DbSet<person_product_set_token> person_product_set_token { get; set; }
        public DbSet<person_product_set_piece> person_product_set_piece { get; set; }
        //public DbSet<person_story_history> person_story_history { get; set; }
        public DbSet<person_achievement> person_achievement { get; set; }
        //public DbSet<person_exchange_product> person_exchange_product { get; set; }
        public DbSet<person_limitbreak> person_limitbreak { get; set; }
        public DbSet<person_levelup> person_levelup { get; set; }
        public PersonDbContext(DbContextOptions<PersonDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 유저 상태
            modelBuilder.Entity<person_status>()
                .HasIndex(s => s.person_id)
                .IsUnique();

            modelBuilder.Entity<person_status>()
                .HasIndex(s => s.email)
                .IsUnique();

            modelBuilder.Entity<person_status>()
                .HasIndex(s => s.display_person_id)
                .IsUnique();

            // 아이템
            modelBuilder.Entity<person_item>()
                .HasIndex(i => new { i.person_id, i.item_id })
                .IsUnique();

            // 캐릭터
            modelBuilder.Entity<person_character>()
                .HasIndex(c => new { c.person_id, c.character_id })
                .IsUnique();

            // 한계돌파 정보
            modelBuilder.Entity<person_limitbreak>()
                .HasIndex(l => new { l.person_id, l.character_id });

            // 업적
            modelBuilder.Entity<person_achievement>()
                .HasIndex(a => new { a.person_id, a.achievement_id })
                .IsUnique();

            // 가챠 뽑기 횟수 (천장/마일리지 등)
            modelBuilder.Entity<person_gacha>()
                .HasIndex(g => new { g.person_id, g.gacha_id });

            // 출석체크
            modelBuilder.Entity<person_loginbonus>()
                .HasIndex(lb => new { lb.person_id, lb.daily_login_bonus_id });

            // 우편함
            modelBuilder.Entity<person_mail>()
                .HasIndex(m => new { m.person_id, m.insert_date });

            modelBuilder.Entity<person_mail>()
                .HasIndex(m => new { m.person_id, m.is_receive });
            
            modelBuilder.Entity<person_login>()
                .HasIndex(l => new { l.person_id, l.insert_date });

            modelBuilder.Entity<person_item_history>()
                .HasIndex(h => new { h.person_id, h.insert_date });
        }
    }
}
