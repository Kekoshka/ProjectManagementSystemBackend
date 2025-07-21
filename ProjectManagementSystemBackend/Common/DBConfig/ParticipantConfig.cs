using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagementSystemBackend.Models;
using System.Reflection.Emit;

namespace ProjectManagementSystemBackend.Common.DBConfig
{
    /// <summary>
    /// Класс для настройки конфигурации участника
    /// </summary>
    public class ParticipantConfig : IEntityTypeConfiguration<Participant>
    {
        /// <summary>
        /// Настраивает связи участника
        /// </summary>
        /// <param name="builder"></param>
        /// <remarks>
        /// Выстраивает связи между участником и созданными задачами,
        /// а также между участником ответственными задачами
        /// </remarks>
        public void Configure(EntityTypeBuilder<Participant> builder)
        {

            builder.HasMany(p => p.CreatedTasks)
                .WithOne(t => t.Creator)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(p => p.ResponsibleTasks)
                .WithOne(t => t.ResponsiblePerson)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
