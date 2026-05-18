using Microsoft.EntityFrameworkCore;
using SharedData.Models;

namespace Empen
{
    public class MasterDbContext : DbContext
    {
        public DbSet<master_gacha> master_gacha { get; set; }
        public DbSet<master_gacha_lot> master_gacha_lot { get; set; }
        public DbSet<master_gacha_lot_group> master_gacha_lot_group { get; set; }
        public DbSet<master_gacha_exec_10> master_gacha_exec_10 { get; set; }
        //public DbSet<master_gacha_bonus_reward_group> master_gacha_bonus_reward_group { get; set; }
        //public DbSet<master_gacha_character_group> master_gacha_character_group { get; set; }
        public DbSet<master_banner> master_banner { get; set; }
        public DbSet<master_item> master_item { get; set; }
        //public DbSet<master_gift> master_gift { get; set; }
        public DbSet<master_mail> master_mail { get; set; }
        public DbSet<master_achievement> master_achievement { get; set; }
        public DbSet<master_achievement_category> master_achievement_category { get; set; }
        public DbSet<master_reward> master_reward { get; set; }
        //public DbSet<master_reward_group> master_reward_group { get; set; }
        //public DbSet<master_story> master_story { get; set; }
        //public DbSet<master_story_group> master_story_group { get; set; }
        //public DbSet<master_story_script> master_story_script { get; set; }
        //public DbSet<master_weekly_mission> master_weekly_mission { get; set; }
        public DbSet<master_character> master_character { get; set; }
        public DbSet<master_daily_login_bonus> master_daily_login_bonus { get; set; }
        public DbSet<master_daily_login_bonus_day> master_daily_login_bonus_day { get; set; }
        public DbSet<master_product> master_product { get; set; }
        public DbSet<master_product_set> master_product_set { get; set; }
        public DbSet<master_product_set_token> master_product_set_token { get; set; }
        public DbSet<master_product_set_prism> master_product_set_prism { get; set; }
        public DbSet<master_product_set_piece> master_product_set_piece { get; set; }
        public DbSet<master_notice> master_notice { get; set; }
        //public DbSet<master_exchange_product_set> master_exchange_product_set { get; set; }
        //public DbSet<master_exchange_product> master_exchange_product { get; set; }
        public DbSet<master_character_level> master_character_level { get; set; }
        public DbSet<master_character_grade> master_character_grade { get; set; }

        public MasterDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // character
            modelBuilder.Entity<master_character>()
                .HasMany(c => c.character_levels)
                .WithOne(l => l.character)
                .HasForeignKey(l => l.character_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<master_character>()
                .HasMany(c => c.character_grades)
                .WithOne(g => g.character)
                .HasForeignKey(g => g.character_id)
                .OnDelete(DeleteBehavior.NoAction);

            // gacha
            modelBuilder.Entity<master_gacha>()
                .HasOne(gacha => gacha.gacha_lot_group)
                .WithMany(group => group.gachas)
                .HasForeignKey(gacha => gacha.gacha_lot_group_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<master_gacha>()
                .HasOne(gacha => gacha.gacha_exec_10)
                .WithMany(exec => exec.gachas)
                .HasForeignKey(gacha => gacha.gacha_exec_10_id)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<master_gacha_lot>()
                .HasOne(l => l.gacha_lot_group)
                .WithMany(lg => lg.gacha_lots)
                .HasForeignKey(l => l.gacha_lot_group_id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<master_gacha_lot>()
                .HasOne(lot => lot.character)
                .WithMany(ch => ch.gacha_lots)
                .HasForeignKey(lot => lot.gacha_character_id)
                .OnDelete(DeleteBehavior.NoAction);

            // loginbonus
            modelBuilder.Entity<master_daily_login_bonus_day>()
                .HasOne(day => day.daily_login_bonus)
                .WithMany(login => login.daily_login_bonus_days)
                .HasForeignKey(day => day.daily_login_bonus_id)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
